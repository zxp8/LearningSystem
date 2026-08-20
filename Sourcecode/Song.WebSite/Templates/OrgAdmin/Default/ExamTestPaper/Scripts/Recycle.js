$ready(['Components/papertype.js'],
    function () {
        window.vapp = new Vue({
            el: '#vapp',
            data: {
                org: window.org,    //当前机构
                types: [],        //试题类型，来自web.config中配置项
                admin: {},          //当前登录用户
                //试题的查询条件
                form: { "orgid": "", "accid": "", "search": "", "isdeleted": true, "diff": "", "use": "", "size": 10, "index": 1 },
                datas: [],
                total: 1, //总记录数
                totalpages: 1, //总页数
                selects: [], //数据表中选中的行

                loadstate: {
                    init: false,        //初始化
                    def: false,         //默认
                    get: false,         //加载数据
                    update: false,      //更新数据
                    del: false          //删除数据
                },
                loadingid: 0,
            },
            mounted: function () {
                this.form.orgid = window.org.Org_ID;
                this.handleCurrentChange(1);
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
                //加载数据页
                handleCurrentChange: function (index) {
                    if (index != null) this.form.index = index;
                    var th = this;
                    //每页多少条，通过界面高度自动计算
                    let area = $dom.height() - 100;
                    th.form.size = Math.floor(area / 64);
                    th.loading = true;
                    var loading = this.$fulloading();
                    $api.get("ExamTestPaper/Pager", th.form).then(function (d) {
                        if (d.data.success) {
                            var result = d.data.result;
                            th.datas = result;
                            th.totalpages = Number(d.data.totalpages);
                            th.total = d.data.total;
                        } else {
                            throw d.data.message;
                        }
                    }).catch(function (err) {
                        alert(err);
                        console.error(err);
                    }).finally(() => {
                        th.loading = false;
                        th.$nextTick(function () {
                            loading.close();
                        });
                    });
                },
                //回收
                recycle: function (btn,datas) {                 
                    var th = this;
                    th.$confirm('是否还原选中的数据?', '提示', {
                        confirmButtonText: '确定',
                        cancelButtonText: '取消',
                        type: 'warning'
                    }).then(() => {
                        th.loadstate.recycle = true;
                        $api.post("ExamTestPaper/Recycle", { "id": datas })
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
                    }).catch(() => { });
                },
                //删除
                deleteData: function (datas) {
                    var th = this; 
                    th.loading = true;
                    $api.delete('ExamTestPaper/Remove', { 'id': datas }).then(function (req) {
                        if (req.data.success) {
                            var result = req.data.result;
                            th.$notify({
                                type: 'success',
                                message: '成功删除' + result + '条数据',
                                center: true
                            });
                            th.handleCurrentChange();
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(function (err) {
                        alert(err);
                        console.error(err);
                    }).finally(() => th.loading = false);
                },                
            },
            filters: {

            },
            components: {

            }
        });
    });