$ready(function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            form: { "orgid": "", "pid": -1, "isuse": "", "isdeleted": true, "name": "", "size": "", "index": 1 },
            datas: [],
            total: 1, //总记录数
            totalpages: 1, //总页数
            selects: [], //数据表中选中的行

            loadingid: 0,
            loadstate: {
                recycle: false,         //还原
                get: false,         //加载数据              
                remove: false          //删除数据
            }
        },
        mounted: function () {

        },
        created: function () {
            var th = this;
            th.form.orgid = window.org.Org_ID;
            th.handleCurrentChange(1);
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
            //加载数据页
            handleCurrentChange: function (index) {
                if (index != null) this.form.index = index;
                var th = this;
                //每页多少条，通过界面高度自动计算
                let area = $dom.height() - 110;
                th.form.size = Math.floor(area / 40);
                th.loadstate.get = true;
                $api.get("ExamQues/KnlPager", th.form).then(function (d) {
                    console.log(3);
                    if (d.data.success) {
                        th.datas = d.data.result;
                        th.totalpages = Number(d.data.totalpages);
                        th.total = d.data.total;
                    } else {
                        throw d.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                }).finally(() => th.loadstate.get = false);
            },
            //回收
            recycle: function (btn, datas) {
                var th = this;
                th.loadstate.recycle = true;
                $api.post("ExamQues/KnlRecycle", { "id": datas })
                    .then(req => {
                        if (req.data.success) {
                            let result = req.data.result;
                            th.handleCurrentChange();
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                    .finally(() => th.loadstate.recycle = false);
            },
            //彻底删除
            remove: function (datas) {
                var th = this;
                th.$confirm('此操作将永久删除该文件, 是否继续?', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'error'
                }).then(() => {
                    th.loadstate.remove = true;
                    $api.delete("ExamQues/KnlRemove", { "id": datas })
                        .then(req => {
                            if (req.data.success) {
                                let result = req.data.result;
                                th.handleCurrentChange();
                            } else {
                                console.error(req.data.exception);
                                throw req.config.way + ' ' + req.data.message;
                            }
                        }).catch(err => console.error(err))
                        .finally(() => th.loadstate.remove = false);
                }).catch(() => { });
            },
        },
        filters: {

        },
        components: {
            //当前试题分类的路径
            'parents_path': {
                //knl : 当前试题分类
                //separator : 分隔符
                props: ['knl', 'separator'],
                data: function () {
                    return {
                        loading: false,
                        paths: [],  //路径
                        showpath: '' //显示路径
                    }
                },
                watch: {
                    knl: {
                        handler: function (val) {
                            this.getPaths();
                        },
                        immediate: true,
                    }
                },
                methods: {
                    //获取路径
                    getPaths: function () {
                        var th = this;
                        th.loading = true;
                        $api.get("ExamQues/KnlParents", { "qkid": th.knl.Qk_ID, "isself": true }).then(function (d) {
                            if (d.data.success) {
                                th.paths = d.data.result;
                                th.showpath = th.calcshowpath(th.paths);
                            } else {
                                throw d.data.message;
                            }
                        }).catch(err => console.error(err))
                            .finally(() => th.loading = false);
                    },
                    //计算显示路径
                    calcshowpath: function (paths) {
                        if (this.separator == null || this.separator == '') this.separator = '/';
                        return paths.map(function (item) {
                            return "<span>" + item.Qk_Name + "</span>";
                        }).join(this.separator);
                    }
                },
                template: `<div class="parents-path">
                   <loading v-if="loading"></loading>
                   <div v-html="showpath"></div>
                </div>`
            },
            //试题总数
            'questotal': {
                props: ['knl'],
                data: function () {
                    return {
                        total: 0,   //试题总数
                        loading: false,
                    }
                },
                watch: {
                    knl: {
                        handler: function (val) {
                            this.getquestotal();
                        },
                        immediate: true,
                    }
                },
                methods: {
                    //获取试题总数
                    getquestotal: function () {
                        var th = this;
                        th.loading = true;
                        $api.get("ExamQues/KnlQusTotal", { "orgid": -1, "qkid": th.knl.Qk_ID, "qtype": "", "use": "", "children": true })
                            .then(req => {
                                if (req.data.success) {
                                    th.total = req.data.result;
                                } else {
                                    console.error(req.data.exception);
                                    throw req.config.way + ' ' + req.data.message;
                                }
                            }).catch(err => console.error(err))
                            .finally(() => th.loading = false);
                    },
                },
                template: `<div>
                    <loading v-if="loading"></loading>
                    <tempalte>{{knl.Qk_Count}} / {{total}}</tempalte>
                </div>`
            }
        }
    });
});