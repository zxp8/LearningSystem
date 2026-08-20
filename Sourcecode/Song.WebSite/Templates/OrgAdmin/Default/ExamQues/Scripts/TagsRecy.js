$ready(function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            //关键字的查询条件
            form: { "orgid": "", "couid": "", "isdeleted": true, "name": "", "size": 8, "index": 1 },
            datas: [],
            total: 1, //总记录数
            totalpages: 1, //总页数 

            loadstate: {
                init: false,        //初始化
                add: false,         //默认              
                remove: false,      //移除
                del: false          //删除数据
            }
        },
        mounted: function () {
            this.handleCurrentChange();
            this.$refs.btngroup.addbtn({
                text: '全选/取消', tips: '选择标签',
                id: 'select', type: 'primary',
                icon: 'a057'
            });
        },
        created: function () {
            var th = this;
            th.form.orgid = window.org.Org_ID;
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
                var th = this;
                if (index != null) this.form.index = index;
                //每页多少条，通过界面高度自动计算
                let area = $dom.height() - 100;
                th.form.size = Math.floor(area / 135) * 5;
                var loading = this.$fulloading();
                $api.get("ExamQues/TagPager", th.form).then(function (d) {
                    if (d.data.success) {
                        var result = d.data.result;
                        //console.error(result);
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
                    th.$nextTick(function () {
                        loading.close();
                    });
                });
            },
            //批量选择
            selectall: function () {
                let isselected = true;   //是否全选
                for (let i = 0; i < this.datas.length; i++) {
                    if (this.datas[i].checked == null || !this.datas[i].checked) {
                        isselected = false;
                        break;
                    }
                }
                for (let i = 0; i < this.datas.length; i++) {
                    this.$set(this.datas[i], 'checked', !isselected);
                }
            },
            //获取选中的id
            getselectid: function () {
                return this.datas.filter(item => item.checked).map(item => item.Qtag_ID);
            },
            //批量删除
            btnbatdel: function (id) {
                if (id != null && id != '') return this.remove(id);
                var arr = this.getselectid();
                if (arr.length < 1) {
                    this.$message({
                        message: '请选中要操作的数据项',
                        type: 'error'
                    });
                    return false;
                }
                var th = this;
                this.$confirm('是否确认删除这 ' + arr.length + ' 项数据? ', '谨慎操作', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'warning'
                }).then(function () {
                    th.remove(arr.join(","));
                }).catch(function () { });
            },
            //彻底删除
            remove: function (datas) {
                var th = this;
                th.$confirm('此操作将永久删除该内容, 是否继续?', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'error'
                }).then(() => {
                    th.loadstate.remove = true;
                    $api.delete("ExamQues/TagRemove", { "id": datas })
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
            //回收
            recycle: function (btn, datas) {
                if (datas == null || datas == '') datas = this.getselectid().join(",");
                if (datas == null || datas == '') {
                    this.$message({
                        message: '请选中要操作的数据项',
                        type: 'error'
                    });
                    return false;
                }
                var th = this;
                let count = datas.split(",").length;
                th.$confirm('是否还原选中的 ' + count + ' 条数据?', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'warning'
                }).then(() => {
                    th.loadstate.recycle = true;
                    $api.post("ExamQues/TagRecycle", { "id": datas })
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
            //标签的颜色
            tagcolor: function (item) {
                let colors = ["info", "success", "warning", "primary", "danger"];
                return colors[Math.floor(item.Qtag_Weight / 2)];
            },
        },
        filters: {

        },
        components: {
            //标签的试题数
            'quescount': {
                props: ['tag', 'orgid'],
                data: function () {
                    return {
                        count: 0,
                        loading: false,
                    }
                },
                watch: {},
                methods: {
                    getcount: function () {
                        var th = this;
                        th.loading = true;
                        $api.get("ExamQues/TagQusTotal", { "qtagid": th.tag.Qtag_ID, "couid": 0, "qtype": -1, "use": null })
                            .then(req => {
                                if (req.data.success) {
                                    th.count = req.data.result;
                                } else {
                                    console.error(req.data.exception);
                                }
                            }).catch(err => console.error(err))
                            .finally(() => th.loading = false);
                    }
                },
                template: `<div class="quescount">
                    <loading v-if="loading"></loading>
                    <template v-else>
                        试题数：{{count}}
                    </template>
                </div>`
            }
        }
    });
});