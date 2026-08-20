$ready(['Components/papertype.js',], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            id: $api.querystring('id'), //主键ID
            testpaper: {},
            form: {},
            //加载状态
            loadstate: {
                init: false,        //初始化
                def: false,         //默认
                get: false,         //加载数据
                update: false,      //更新数据
                del: false          //删除数据
            }
        },
        mounted: function () {
            this.getentity();
            //this.gototype(2);
        },
        created: function () {

        },
        computed: {
            //加载状态
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
            //试卷对象是否为空
            isnull: t => $api.isnull(t.testpaper),
        },
        watch: {
            //选择时间区间
            'id': {
                handler: function (nv, ov) {

                },
                immediate: true,
                deep: true
            }
        },
        methods: {
            //跳转到指定类型页面, type:1静态试卷，2动态随机试卷
            gototype: function (type) {
                let typepurl = [{
                    page: 'ModifyTestpaper1',       //固定试题的试卷
                    width: 1200, height: '80%'
                }, {
                    page: 'ModifyTestpaper2',       //随机出题的试卷
                    width: 1000, height: 800
                }];

                //控制弹窗大小
                let $pagebox = window.top.$pagebox;
                if ($pagebox) {
                    let pbox = $pagebox.get(window.name);
                    if (!pbox.full) pbox.toSize(typepurl[type - 1].width, typepurl[type - 1].height);
                }
                //跳转到指定页面
                let url = $api.url.set(typepurl[type - 1].page, { 'id': this.id });
                window.location.href = url;
            },
            //获取试卷
            getentity: function () {
                var th = this;
                if (th.isadd) return;
                th.loadstate.get = true;
                $api.get("ExamTestPaper/ForID", { "id": th.id })
                    .then(req => {
                        if (req.data.success) {
                            th.testpaper = req.data.result;
                            if (!th.isnull) {
                                th.gototype(th.testpaper.Etp_Type);
                            }
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                    .finally(() => th.loadstate.get = false);
            }
        },
        filters: {

        },
        components: {

        }
    });
});