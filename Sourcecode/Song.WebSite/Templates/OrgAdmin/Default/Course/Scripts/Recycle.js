$ready(['Components/course_data.js',
    'Components/course_income.js',
    'Components/course_prices.js'], function () {
        window.vapp = new Vue({
            el: '#vapp',
            data: {
                org: {},
                form:
                {
                    'orgid': '', 'sbjids': '', 'thid': '', 'use': '', 'del': true, 'live': '', 'free': '',
                    'search': '', 'order': 'def',
                    'size': 1, 'index': 1
                },
                datas: [],
                total: 1, //总记录数
                totalpages: 1, //总页数
                selects: [], //数据表中选中的行
                exception: '',

                teachers: [],        //教师

                loadingid: 0,
                loadstate: {
                    recycle: false,         //还原
                    get: false,         //加载数据      
                    teacher: false,         //还原        
                    remove: false          //删除数据
                }
            },
            mounted: function () {

            },
            created: function () {
                var th = this;
                th.org = window.org;
                th.form.orgid = window.org.Org_ID;
                th.handleCurrentChange(1);
                th.teacher_query('');
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
                //列表数据为空时显示提示
                'table_empty': function () {
                    return this.exception != '' ? this.exception : '暂无数据';
                },
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
                    th.form.size = Math.floor(area / 57);
                    th.loadstate.get = true;
                    th.exception = '';
                    $api.get("Course/Pager", th.form).then(function (d) {
                        if (d.data.success) {
                            th.datas = d.data.result;
                            //console.log(th.datas);
                            th.totalpages = Number(d.data.totalpages);
                            th.total = d.data.total;
                        } else {
                            console.error(d.data.exception);
                            th.exception = d.data.message;
                            throw d.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loadstate.get = false);
                }, //获取教师列表
                teacher_query: function (search) {
                    var th = this;
                    th.loadstate.teacher = true;
                    let query = {
                        orgid: th.org.Org_ID, titid: '', gender: '-1', isuse: '',
                        search: search, phone: '', acc: '', idcard: '',
                        order: 'pinyin', size: 9999999, index: 1
                    };
                    $api.get('Teacher/Pager', query).then(function (req) {
                        if (req.data.success) {
                            th.teachers = req.data.result;
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loadstate.teacher = false);

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
                        $api.post("Course/Recycle", { "id": datas })
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
                        $api.delete("Course/Remove", { "id": datas })
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
    });