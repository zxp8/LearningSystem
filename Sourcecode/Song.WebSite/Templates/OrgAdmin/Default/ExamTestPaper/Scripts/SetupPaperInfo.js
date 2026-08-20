$ready(function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            types: [],       //题型
            //试卷对象  
            entity: {},
            //图片文件
            upfile: null, //本地上传文件的对象      
            Etp_Diff: [1, 5],     //难度范围   
            tabs: [
                { name: '基本信息', tab: 'general', icon: 'e72f' },
                { name: '简介', tab: 'intro', icon: 'e6cb' },
                { name: '其它', tab: 'other', icon: 'e67e' },
            ],
            activeName: 'general',
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
                    { required: true, message: '时长不得为空', trigger: 'blur' },
                    {
                        validator: function (rule, value, callback) {
                            if (!(/^[1-9]\d*$/.test(value))) return callback(new Error('请输入大于零的整数'));;
                            if (Number(value) > 600) return callback(new Error('时长不能超过6小时'));;
                            callback();
                        }, trigger: 'blur'
                    }
                ],
            },
            loadstate: {
                init: false,        //初始化
                def: false,         //默认
                get: false,         //加载数据
                update: false,      //更新数据
                del: false          //删除数据
            }
        },
        mounted: function () {
            this.receive().then(d => {
                this.entity = $api.clone(d);
            });
        },
        created: function () {

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
            }
        },
        watch: {

        },
        methods: {
            //接收的主窗体数据
            receive: function () {
                return new Promise((resolve, reject) => {
                    //像主窗体传值，传三个值：选中的分类，选中的试题数，调用函数名
                    var pagebox = window.top.$pagebox;
                    if (pagebox && pagebox.source.top) {
                        [this.entity, this.types, this.upfile] = pagebox.source.box(window.name, 'vapp.transmitInfo', false);
                        this.Etp_Diff = [this.entity.Etp_Diff, this.entity.Etp_Diff2];
                        resolve(this.entity);
                    }
                });
            },
            //确认操作，保存数据
            btnEnter: function (formName, isclose) {
                var th = this;
                this.$refs[formName].validate((valid, fields) => {
                    if (valid) {
                        //像主窗体传值，当前实体，图片对象
                        var pagebox = window.top.$pagebox;
                        if (pagebox && pagebox.source.box) {
                            pagebox.source.box(window.name, 'vapp.receiveInfo', false, [th.entity, th.upfile]);
                            let curbox = pagebox.source.self(window.name);
                            curbox.shut();
                        }

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
        },
        filters: {

        },
        components: {

        }
    });
});