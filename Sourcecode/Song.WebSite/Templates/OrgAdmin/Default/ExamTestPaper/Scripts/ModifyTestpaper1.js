$ready([
    '/Utilities/Components/question/function.js',
    '../Question/Components/ques_type.js',
    'Components/editbyname.js',     //修改试题类型的别名
    'Components/papertype.js',  //试卷类型
    'Components/quesrow.js',     //试题在试卷中的显示
    'Components/quesscores.js'   //试题的分数
], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            id: $api.querystring('id'), //主键ID
            org: {},          //当前机构对象
            config: {},      //当前机构配置项  
            types: [],       //题型
            //试题的类型数据，例如题型，该题型的题量，分数，分数占比，
            qtypeitems: [],
            //图片文件
            upfile: null, //本地上传文件的对象         
            Etp_Diff: [1, 5],     //难度范围
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
                Etp_Remind: '',
                Etp_Count: 0,       //总题量                   
            },
            //录入校验的规划
            rules1: {
                Etp_Name: [
                    { required: true, message: '试卷名称不得为空', trigger: 'blur' },
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
                ]
            },
            rules2: {
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
                    { required: true, message: '及格分不得为空', trigger: 'blur' },
                    {
                        validator: function (rule, value, callback) {
                            if (!(/^[1-9]\d*$/.test(value))) return callback(new Error('请输入大于零的整数'));
                            if (Number(value) > vapp.entity.Etp_Total) callback(new Error('及格分不得大于满分'));
                            else callback();

                        }, trigger: 'blur'
                    }
                ]
            },
            rules3: {},
            loadstate: {
                init: false,        //初始化
                def: false,         //默认
                get: false,         //加载数据
                update: false,      //更新数据
                del: false          //删除数据
            }
        },
        updated: function () {
            this.$mathjax();
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
                        number: 0,       //分数
                        percent: 0,   //分数占比
                        ques: []     //题型下的试题
                    }
                });
                //添加题型录入的验证方法
                for (var i = 0; i < th.qtypeitems.length; i++) {
                    const item = th.qtypeitems[i];
                    //题型分数点比的验证
                    let rulepercent = [{
                        validator: function (rule, value, callback) {
                            let field = rule.field;
                            let type = Number(field.substring(field.length - 1));   //题型
                            let item = vapp.qtypeitems.find(i => i.type == type);
                            if (item.percent < 0) return callback(new Error('请输入大于零的整数'));
                            //分数占比大于零，试题不得为零
                            if (item.percent == 0 && item.ques.length > 0) return callback(new Error("不可为零"));
                            //分数占比大于零，试题不得为零
                            if (item.percent > 0 && item.ques.length <= 0) return callback(new Error("不可大于零"));
                            //
                            let total = vapp.qtypeitems.reduce((a, b) => a + b.percent, 0);
                            if (total != 100) return callback(new Error("分数占比之和必须为100"));

                            callback();

                        }, trigger: 'blur'
                    }];
                    th.$set(th.rules3, 'percent' + item.type, rulepercent);
                }
                if (!th.isadd) th.getentity();
            }).catch(err => console.error(err))
                .finally(() => th.loadstate.init = false);
        },
        created: function () {
            //更改当前窗体的标题
            var pagebox = window.top.$pagebox;
            if (pagebox && pagebox.source) {
                let box = pagebox.source.self(window.name);
                box.title = box.title.substring(0, box.title.lastIndexOf('-') + 1) + ' 固定试题';
                window.setTimeout(function () {
                    //box.full = true;
                }, 200);
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
            //各题型占比总和
            percenttotal: function () {
                return this.qtypeitems.reduce((a, b) => a + b.percent, 0);
            }
        },
        watch: {

        },
        methods: {
            //获取试卷的数据实体
            getentity: function () {
                if (this.isadd) return;
                var th = this;
                th.loadstate.get = true;
                $api.get("ExamTestPaper/ForDetails1", { "id": th.id })
                    .then(req => {
                        if (req.data.success) {
                            let result = req.data.result;
                            th.entity = result.paper;
                            //试题题型分配数据
                            let questions = result?.questions ?? [];
                            for (let i = 0; i < questions.length; i++) {
                                const ques = questions[i];
                                const item = th.qtypeitems.find(el => Number(el.type) == Number(ques.type));
                                ques.total = Number(ques.total);
                                ques.count = Number(ques.count);
                                ques.number = Number(ques.number);
                                ques.percent = Number(ques.percent);
                                //解析答案，将xml转为json
                                for (let j = 0; j < ques.ques.length; j++)
                                    ques.ques[j] = window.ques.parseAnswer(ques.ques[j]);
                            }

                            th.qtypeitems = questions.length < 1 ? th.qtypeitems : questions;
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                    .finally(() => th.loadstate.get = false);
            },
            //打开试卷基础信息的页面
            openpaperinfo: function () {
                if (!window.top.$pagebox) return;
                let page = 'SetupPaperInfo'
                var suburl = $dom.routepath() + page;    //子窗口页面路径                   
                var curbox = window.top.$pagebox.get(window.name);   //当前窗口
                //创建新窗口中
                var subbox = window.top.$pagebox.create({
                    height: 600, id: page, ico: 'e667', title: '设置试卷信息 - 固定试卷',
                    url: suburl
                });
                curbox.opensub(subbox, 'top');
            },
            //向“更多分数设置”的窗体传递数据
            transmitInfo: function () {
                //试卷对象，题型
                return [this.entity, this.types, this.upfile];
            },
            //接收子窗体（基本信息编辑）的数据
            receiveInfo: function ([entity, upfile]) {
                this.entity.Etp_Name = entity.Etp_Name;
                this.entity.Etp_SubName = entity.Etp_SubName;
                this.entity.Etp_Span = entity.Etp_Span;
                this.entity.Etp_Remind = entity.Etp_Remind;
                this.entity.Etp_Intro = entity.Etp_Intro;
                this.entity.Etp_Diff = entity.Etp_Diff;
                this.entity.Etp_Diff2 = entity.Etp_Diff2;
                this.upfile = upfile;
            },
            /***************************
             * 试卷分数设置
             */
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
                this.$refs['form2'].validate();
                this.$refs['form3'].validate();
            },
            /***************************
             * 试题编辑
             */
            //移动题型顺序
            typemove: function (index, direction) {
                //console.error(index);
                const newArr = [...this.qtypeitems];
                const element = newArr.splice(index, 1)[0];
                newArr.splice(index + direction, 0, element);
                this.qtypeitems = newArr;
            },
            //打开选择试题的子窗体
            openselques: function (type) {
                if (!window.top.$pagebox) return;
                let item = this.qtypeitems.find(el => el.type == type);
                let page = 'SelectQuestions';
                let suburl = $dom.routepath() + page;    //子窗口页面路径      
                suburl = $api.url.set(suburl,
                    {
                        'type': item.type,
                        'types': this.types.join(',')
                    });
                var curbox = window.top.$pagebox.get(window.name);   //当前窗口
                //创建新窗口中
                var subbox = window.top.$pagebox.create({
                    width: 1000, id: page, ico: 'e755', title: '选择 ' + item.name + '题',
                    url: suburl
                });
                curbox.opensub(subbox, 'left');
            },
            //向“选择试题”的窗体传递数据
            transmitques: function (type) {
                let item = this.qtypeitems.find(el => Number(el.type) == Number(type));
                //试卷对象，题型，题型分数分配的数据
                return [item.ques];
            },
            //接收子窗体（试题选择）的数据
            //ques:试题列表
            //type:题型
            receiveques: function ([ques, type]) {
                let item = this.qtypeitems.find(el => Number(el.type) == Number(type));
                item.ques = [];
                item.ques = ques;
                item.count = ques.length;
                //console.error(this.qtypeitems);
            },
            /***************************
             * 试卷保存
             */
            //确认操作
            btnEnter: async function (formName, isclose) {
                try {
                    // 同时验证所有表单
                    const results = await Promise.all([
                        this.$refs.form1.validate(),
                        this.$refs.form2.validate(),
                        this.$refs.form3.validate()
                    ]);
                    this.submitData(isclose)
                } catch (error) {
                    console.log('表单验证失败');

                    this.$message.error('表单验证失败');
                    return false
                }
            },
            //提交数据
            submitData: function (isclose) {
                console.error('录入验证通过');
                var th = this;
                let xml = th.buildXml();
                th.entity.Etp_FromConfig = xml;
                th.entity.Etp_Type = 1;
                th.entity.Etp_Count = th.qtypeitems.reduce((total, item) => total + item.ques.length, 0);
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
            },
            //生成xml数据，格式说明参考同文件夹下的ModifyTestpaper2.xml
            buildXml: function () {
                var count = 0;
                var xml = '<testpaper type="1">';
                //题型分数分配
                xml += '<questions>'
                let items = this.qtypeitems;
                for (let i = 0; i < items.length; i++) {
                    const m = items[i];
                    xml += '<ques type="' + m.type + '" name="' + m.name + '" byname="' + m.byname + '"'
                        + ' number="' + m.number + '"'
                        + ' count="' + m.count + '"'
                        + ' percent="' + m.percent + '"'
                    xml += ' >';
                    for (let j = 0; j < m.ques.length; j++) {
                        const q = m.ques[j];
                        xml += '<q id="' + q.Qus_ID + '"/>';
                    }
                    xml += '</ques>';
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
                    pagebox.source.tab(window.name, 'vapp.fresh_row', isclose, this.id);
            }
        },
        filters: {
            //汉字序号，例如一、二、
            chiserial: function (val) {
                let arr = ['零', '一', '二', '三', '四', '五', '六', '七', '八', '九'];
                return arr[val + 1] + '、';
            }
        },
        components: {

        }
    });
});