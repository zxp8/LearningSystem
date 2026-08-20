$ready(['../Question/Components/ques_type.js',
    'Components/papertype.js'],
    function () {
        window.vapp = new Vue({
            el: '#vapp',
            data: {
                id: $api.querystring('id'), //主键ID
                org: {},          //当前机构对象
                config: {},      //当前机构配置项  
                types: [],       //题型
                tabs: [
                    { name: '基本信息', tab: 'general', icon: 'e72f' },
                    { name: '出题&计分', tab: 'range', icon: 'e731' },
                    { name: '简介', tab: 'intro', icon: 'e6cb' },
                    { name: '其它', tab: 'other', icon: 'e67e' },
                    //{ name: '帮助说明', tab: 'help', icon: 'a026' },
                ],
                activeName: 'general',
                //试卷对象  
                entity: {
                    Etp_Id: 0,        //主键
                    Etp_Name: '', Etp_SubName: '',
                    Etp_IsUse: true,
                    Etp_Span: 120,    //默认限时 120分钟
                    Etp_Type: 2,
                    Etp_Total: 100, Etp_PassScore: 60,      //总分，及格分
                    Etp_Diff: 1,
                    Etp_Diff2: 5,
                    Etp_FromConfig: '',
                    Etp_Count: 0,       //总题量                   
                },
                //试题的类型数据，例如题型，该题型的题量，分数，分数占比，
                qtypeitems: [],
                //图片文件
                upfile: null, //本地上传文件的对象         
                Etp_Diff: [1, 5],     //难度范围

                //试题选择范围
                parts: [],      //关联的试题分类
                tags: [],       //关联的关键字
                knls: [],       //关联的知识点
                //各个选择范围的试题数量
                rangeques: {
                    part: 0, tag: 0, knl: 0
                },
                //供出卷的试题数量
                quescount: {
                    part: 0, tag: 0, knl: 0, total: 0
                },

                //录入校验的规划
                rules: {
                    Etp_Name: [
                        { required: true, message: '标题不得为空', trigger: 'blur' },
                        { min: 1, max: 255, message: '最长输入255个字符', trigger: 'change' },
                        { validator: validate.name.proh, trigger: 'change' },   //禁止使用特殊字符
                        { validator: validate.name.danger, trigger: 'change' },
                        {
                            validator: function (rule, value, callback) {
                                let v = $api.trim(value);
                                if (v == '' || v.length < 1) return callback(new Error('不能全部是空格'));
                                return callback();
                            }, trigger: 'blur'
                        }
                    ],
                    Etp_Span: [
                        { required: true, message: '限时不得为空', trigger: 'blur' },
                        {
                            validator: function (rule, value, callback) {
                                if (!(/^[1-9]\d*$/.test(value))) return callback(new Error('请输入大于零的整数'));;
                                if (Number(value) < Number(vapp.entity.Etp_PassScore))
                                    return callback(new Error('试卷总分不得小于及格分'));;
                                callback();
                            }, trigger: 'blur'
                        }
                    ],
                    Etp_Total: [
                        { required: true, message: '分数不得为空', trigger: 'blur' },
                        {
                            validator: function (rule, value, callback) {
                                if (/^[1-9]\d*$/.test(value)) return callback();
                                callback(new Error('请输入大于零的整数'));
                            }, trigger: 'blur'
                        }
                    ],
                    Etp_PassScore: [
                        { required: true, message: '分数不得为空', trigger: 'blur' },
                        {
                            validator: function (rule, value, callback) {
                                if (!(/^[1-9]\d*$/.test(value))) return callback(new Error('请输入大于零的整数'));
                                if (Number(value) > vapp.entity.Etp_Total) callback(new Error('及格分不得大于满分'));
                                else callback();

                            }, trigger: 'blur'
                        }
                    ],
                    total: [
                        {
                            validator: function (rule, value, callback) {
                                let items = vapp.qtypeitems;
                                let percent = items.reduce((a, b) => a + b.percent, 0);
                                if (percent > 100) return callback(new Error('各题型分数占比之和不能大于100'));
                                if (percent != 100) return callback(new Error('各题型分数占比之和必须等于100'));

                                for (let i = 0; i < items.length; i++) {
                                    const el = items[i];
                                    if (el.count > el.total) {
                                        return callback(new Error('' + el.name + '题的数量不能大于' + el.total + '道'));
                                    }
                                    if (el.count <= 0 && el.percent > 0) {
                                        return callback(new Error('' + el.name + '题的数量不能小于1道'));
                                    }
                                    if (el.percent <= 0 && el.count > 0) {
                                        return callback(new Error('' + el.name + '题的分数不可小于1分'));
                                    }
                                }
                                callback();
                            }, trigger: 'blur'
                        }
                    ]
                },
                loadstate: {
                    init: false,        //初始化
                    def: false,         //默认
                    get: false,         //加载数据
                    update: false,      //更新数据
                    total: false          //获取试题数量
                }
            },
            mounted: function () {
                var th = this;
                th.org = window.org;
                th.config = window.config;
                //获取题型
                th.loadstate.init = true;
                $api.bat(
                    $api.cache('Question/Types:99999')
                ).then(([types]) => {
                    th.types = types.data.result;
                    //各题型的题量分数的配置
                    th.qtypeitems = th.types.map((t, i) => {
                        return {
                            type: i + 1,  //题型，数字表示
                            name: t,    //题型名称
                            byname: '',    //题型的别名
                            total: 0,       //可供选择的题量
                            count: 0,        //题量
                            number: 0,       //题型的分数
                            percent: 0,   //分数占比
                        }
                    });
                    th.getquestotal();
                    if (!th.isadd) th.getentity();
                }).catch(err => console.error(err))
                    .finally(() => th.loadstate.init = false);
            },
            created: function () {
                //更改当前窗体的标题
                var pagebox = window.top.$pagebox;
                if (pagebox && pagebox.source) {
                    let box = pagebox.source.self(window.name);
                    box.title = box.title.substring(0, box.title.lastIndexOf('-') + 1) + ' 随机出题';
                }
            },
            computed: {
                loading: function () {
                    if (!this.loadstate) return false;
                    for (let key in this.loadstate) {
                        if (this.loadstate.hasOwnProperty(key)
                            && this.loadstate[key])
                            return true;
                    }
                    return false;
                },
                //是否为新增
                isadd: t => t.id == null || t.id == '' || this.id == 0,
                //是否存在试卷对象
                exist: t => !$api.isnull(t.entity) && t.entity.Etp_Id != '',
                //试题的选择范围，这里是临时变量，用时监听选择范围是否变化
                range: function () {
                    return { a: this.parts, b: this.tags, c: this.knls };
                },
            },
            watch: {
                //当出题范围发生变化时
                range: {
                    handler: function (v) {
                        this.getquestotal();
                    },
                },
            },
            methods: {
                //获取试卷的数据实体
                getentity: function () {
                    if (this.isadd) return;
                    var th = this;
                    th.loadstate.get = true;
                    $api.get("ExamTestPaper/ForDetails2", { "id": th.id })
                        .then(req => {
                            if (req.data.success) {
                                let result = req.data.result;
                                th.entity = result.paper;
                                th.parts = result?.parts ?? [];
                                th.knls = result?.knls ?? [];
                                th.tags = result?.tags ?? [];
                                //试题题型分配数据
                                let questions = result?.questions ?? [];
                                for (let i = 0; i < questions.length; i++) {
                                    const ques = questions[i];
                                    const item = th.qtypeitems.find(el => Number(el.type) == Number(ques.type));
                                    ques.total = Number(item.total);
                                    ques.count = Number(ques.count);
                                    ques.number = Number(ques.number);
                                    ques.percent = Number(ques.percent);
                                }
                                th.qtypeitems = questions.length<1 ? th.qtypeitems : questions;                                
                                //试卷出卷范围的题量，如选中的试题分类的试题数
                                th.quescount = result.quescount;
                            } else {
                                console.error(req.data.exception);
                                throw req.config.way + ' ' + req.data.message;
                            }
                        }).catch(err => console.error(err))
                        .finally(() => th.loadstate.get = false);
                },
                //难度范围变更时
                diffChange: function (val) {
                    this.entity['Etp_Diff'] = val[0];
                    this.entity['Etp_Diff2'] = val[1];
                },
                //打开子窗口
                //page:页面名称，
                opensubwin: function (page, title, icon, width, position) {
                    if (!window.top.$pagebox) return;
                    //子窗口页面路径
                    var suburl = $dom.routepath() + page;
                    if (page.toLowerCase().indexOf('part') > -1)
                        suburl = $api.url.set(suburl, { 'id': this.parts.map(p => p.Qp_ID).join(',') });
                    if (page.toLowerCase().indexOf('knl') > -1)
                        suburl = $api.url.set(suburl, { 'id': this.knls.map(p => p.Qk_ID).join(',') });
                    if (page.toLowerCase().indexOf('tag') > -1)
                        suburl = $api.url.set(suburl, { 'id': this.tags.map(p => p.Qtag_ID).join(',') });
                    //当前窗口
                    var curbox = window.top.$pagebox.get(window.name);
                    //创建新窗口中
                    var subbox = window.top.$pagebox.create({
                        width: width, height: 300,
                        id: page, ico: icon, title: title,
                        url: suburl
                    });
                    //打开子窗口,窗口位置:left,right,top,bottom
                    if (position == null) position = 'left';
                    curbox.opensub(subbox, position);
                },
                //接收子窗口数据
                //data:子窗口返回的数据
                //func:要调用的函数名称
                receive: function ([data, count, func]) {
                    if (func == 'selectpart') [this.parts, this.quescount.part] = [data, count];
                    if (func == 'selectknl') [this.knls, this.quescount.knl] = [data, count];
                    if (func == 'selecttag') [this.tags, this.quescount.tag] = [data, count];
                    //
                    this.getquestotal();
                },
                //获取可供选择的试题数量
                getquestotal: function () {
                    var th = this;
                    th.loadstate.total = true;
                    let form = { "orgid": "", "qpid": "", "tagid": "", "knlid": "", "isdeleted": false, "diffs": "", "use": true, "error": false, "wrong": false };
                    form.orgid = th.org.Org_ID;
                    form.qpid = th.parts.map(p => p.Qp_ID).join(',');
                    form.tagid = th.tags.map(p => p.Qtag_ID).join(',');
                    form.knlid = th.knls.map(p => p.Qk_ID).join(',');
                    $api.get("ExamQues/QuesTotal", form)
                        .then(req => {
                            if (req.data.success) {
                                var result = req.data.result;
                                for (let i = 0; i < this.qtypeitems.length; i++) {
                                    const el = this.qtypeitems[i];
                                    el.total = result[i].total;
                                    th.$set(el, 'total', result[i].total);
                                    th.$set(this.qtypeitems, i, el);
                                }
                                th.quescount.total = result.reduce((total, item) => total + item.total, 0);
                            } else {
                                console.error(req.data.exception);
                                throw req.config.way + ' ' + req.data.message;
                            }
                        }).catch(err => console.error(err))
                        .finally(() => th.loadstate.total = false);
                },
                //向“更多分数设置”的窗体传递数据
                scoretransmit: function () {
                    //console.error('scoretransmit');
                    //试卷对象，题型，题型分数分配的数据
                    return [this.entity, this.types, this.qtypeitems];
                },
                //接收“更多分数设置”的窗体数据
                scorereceive: function ([entity, qtypeitems]) {
                    this.entity = entity;
                    this.qtypeitems = qtypeitems;
                    // /console.error('来自子窗体数据');
                },
                //当试卷总分更改时
                chanageTotal: function () {
                    let tptotal = this.entity.Etp_Total;
                    let tmscore = 0;
                    this.qtypeitems.forEach(el => {
                        el.number = Math.floor(el.percent * tptotal / 100);
                        tmscore += el.number;
                    });
                    //重新计算各题型占比的总和
                    let percenttotal = this.qtypeitems.reduce((a, b) => a + b.percent, 0);
                    if (percenttotal == 100) {
                        if (tmscore - tptotal > 0) {
                            const maxel = this.qtypeitems.reduce((p, c) => p.number > c.number ? p : c);
                            this.$set(maxel, 'number', maxel.number - (tmscore - tptotal));
                        } else {
                            const minxel = this.qtypeitems.reduce((p, c) => p.number < c.number ? p : c);
                            this.$set(minxel, 'number', minxel.number - (tmscore - tptotal));
                        }
                    }
                    this.$refs['form'].validate();
                },
                //确认操作，保存数据
                btnEnter: function (formName, isclose) {
                    var th = this;
                    this.$refs[formName].validate((valid, fields) => {
                        if (valid) {
                            console.log('检验通过');
                            let xml = th.buildXml();
                            th.entity.Etp_FromConfig = xml;
                            th.entity.Etp_Type = 2;
                            th.entity.Etp_Count = th.qtypeitems.reduce((total, item) => total + item.count, 0);
                            //console.error(xml);
                            let apipath = th.isadd ? 'ExamTestPaper/Add' : 'ExamTestPaper/Modify';
                            //接口参数，如果有上传文件，则增加file
                            var para = { 'entity': th.entity };
                            if (th.upfile != null) para['file'] = th.upfile;
                            th.loadstate.update = true;
                            $api.post(apipath, para).then(function (req) {
                                if (req.data.success) {
                                    var result = req.data.result;
                                    th.$message({
                                        type: 'success', center: true,
                                        message: '操作成功!'
                                    });
                                    th.operateSuccess(isclose);
                                } else {
                                    throw req.data.message;
                                }
                            }).catch(function (err) {
                                alert(err, '错误');
                            }).finally(() => th.loadstate.update = false);

                        } else {
                            //如果验证未通过，则显示输入项所在的选项卡
                            th.$nextTick(() => {
                                console.error('录入验证失败');
                                let err = $dom('.el-form-item.is-error').first();
                                if (err && err.length > 0) {
                                    while (err.attr('tab') == null) err = err.parent();
                                    this.activeName = err.attr('tab');
                                }
                            });

                        }
                    });
                },
                //生成xml数据，格式说明参考同文件夹下的ModifyTestpaper2.xml
                buildXml: function () {
                    var count = 0;
                    var xml = '<testpaper type="2">';
                    //抽题范围
                    xml += '<range>';
                    xml += '<parts>' + this.parts.map(p => p.Qp_ID).join(',') + '</parts>';
                    xml += '<knls>' + this.knls.map(p => p.Qk_ID).join(',') + '</knls>';
                    xml += '<tags>' + this.tags.map(p => p.Qtag_ID).join(',') + '</tags>';
                    xml += '</range>'
                    //题型分数分配
                    xml += '<questions>'
                    let items = this.qtypeitems;
                    for (let i = 0; i < items.length; i++) {
                        const m = items[i];
                        xml += '<ques type="' + m.type + '" name="' + m.name + '" byname="' + m.byname + '"'
                            + ' number="' + m.number + '"'
                            + ' count="' + m.count + '"'
                            + ' percent="' + m.percent + '"'
                        xml += ' />';
                    }
                    xml += '</questions>';
                    xml += '</testpaper>';
                    return xml;
                },
                //操作成功
                operateSuccess: function (isclose) {
                    //如果处于课程编辑页，则刷新
                    var pagebox = window.top.$pagebox;
                    if (pagebox && pagebox.source.top)
                        pagebox.source.tab(window.name, 'vapp.fresh_row', isclose,this.id);
                }
            },
            filters: {

            },
            components: {

            }
        });
    });