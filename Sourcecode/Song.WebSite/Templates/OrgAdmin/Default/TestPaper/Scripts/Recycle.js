$ready([], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            org: {},
            config: {},      //当前机构配置项    
            types: [],        //试题类型，来自web.config中配置项

            //所有课程
            courses_all: [],
            couid: '',
            //试卷的查询条件
            form: {
                'orgid': '', 'sbjid': '', 'couid': '',
                'search': '', 'isuse': '', 'del': true, 'diff': '', 'size': 20, 'index': 1
            },

            datas: [],
            total: 1, //总记录数
            totalpages: 1, //总页数
            selects: [], //数据表中选中的行
            exception: '',

            loadingid: 0,
            loadstate: {
                init: false,        //初始化
                def: false,         //默认
                get: false,         //加载数据
                update: false,      //更新数据
                del: false          //删除数据
            }
        },
        mounted: function () {

        },
        created: function () {
            var th = this;
            th.org = window.org;
            th.form.orgid = th.org.Org_ID;
            th.config = window.config;
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
                let area = $dom.height() - 100;
                th.form.size = Math.floor(area / 65);
                th.loadstate.get = true;
                th.exception = '';
                var loading = this.$fulloading();
                $api.get("TestPaper/Pager", th.form).then(function (d) {
                    if (d.data.success) {
                        var result = d.data.result;
                        th.datas = result;
                        th.totalpages = Number(d.data.totalpages);
                        th.total = d.data.total;
                    } else {
                        console.error(d.data.exception);
                        th.exception = d.data.message;
                        throw d.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => {
                        th.loadstate.get = false;
                        th.$nextTick(function () {
                            loading.close();
                        });
                    });
            },
            //回收
            recycle: function (btn, datas) {
                var th = this;
                th.$confirm('是否还原选中的数据?', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'warning'
                }).then(() => {
                    th.loadstate.recycle = true;
                    $api.post("TestPaper/Recycle", { "id": datas })
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
            //彻底删除
            remove: function (datas) {
                var th = this;
                th.$confirm('此操作将永久删除该内容, 是否继续?', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'error'
                }).then(() => {
                    th.loadstate.remove = true;
                    $api.delete("TestPaper/Remove", { "id": datas })
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

        }
    });
    //显示课程名称
    Vue.component('course_name', {
        //couid:当前试题的id
        //courses：所有课程
        props: ["couid", "courses", "index"],
        data: function () {
            return {
                course: {},
                loading: false
            }
        },
        watch: {
            'couid': {
                handler: function (nv) {
                    if (!$api.isnull(nv)) {
                        if (this.index == 0) this.startInit();
                    }
                }, immediate: true
            }
        },
        computed: {},
        mounted: function () { },
        methods: {
            //初始加载
            startInit: function () {
                //加载完成，则加载后一个组件，实现逐个加载的效果
                this.getcourse().finally(() => {
                    var vapp = window.vapp;
                    var ctr = vapp.$refs['couname_' + (this.index + 1)];
                    if (ctr != null) {
                        window.setTimeout(ctr.startInit, 50);
                    }
                });
            },
            //获取课程信息
            getcourse: function () {
                var th = this;
                return new Promise(function (res, rej) {
                    if (th.courses) {
                        th.course = th.courses.find(item => item.Cou_ID == th.couid);
                        if (th.course != undefined) return res();
                    }
                    th.loading = true;
                    $api.cache('Course/ForID', { 'id': th.couid }).then(function (req) {
                        if (req.data.success) {
                            th.course = req.data.result;
                        } else {
                            th.loading = false;
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => {
                            th.loading = false;
                            return res();
                        });
                });
            }
        },
        template: `<span>
            <i class="el-icon-loading" v-if="loading"></i>
            <icon v-else-if="course" course>{{course.Cou_Name}}</icon>
        </span>`
    });
});