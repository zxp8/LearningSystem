
$ready(['../Question/Components/ques_type.js',
    'Components/selectpart.js',
    'Components/selectknl.js',
    'Components/selecttag.js',],
    function () {
        window.vapp = new Vue({
            el: '#vapp',
            data: {
                org: {},
                config: {},      //当前机构配置项    
                types: [],        //试题类型，来自web.config中配置项

                //查询条件
                form: { "orgid": "", "types": [], "qpid": "", "tagid": "", "knlid": "", "isdeleted": false, "diffs": [], "use": true, "error": "", "wrong": "" },
                subpath: 'ExamQuesToExcel',  //导出文件的路径，相对临时路径的子路径    
                parts: [],  //选中的试题分类
                knls: [],
                tags: [],

                rules: {
                    types: [
                        { required: true, message: '必须选择一个试题类型', trigger: 'change' }
                    ],
                    diffs: [
                        { required: true, message: '必须选择一个难度', trigger: 'change' }
                    ]
                },
                //一些面板的显示
                showpanel: {
                    parts: false, knls: false, tags: false
                },
                //试题总记录
                questotal: 0,       //总记录数
                loading_total: false,   //获取试题数的加载中

                loading: false,
                loading_export: false,       //生成的预载

                files: [],
                filepanel: false      //显示文件列表的面板
            },
            created: function () {
                var th = this;
                th.org = window.org;
                th.config = window.config;
                th.form.orgid = th.org.Org_ID;
                $api.cache('Question/Types:99999').then(types => th.types = types.data.result);
                //获取已经导出的文件
                this.getFiles();
            },
            watch: {
                //监听表单数据变化
                'form': {
                    handler: function (val) {
                        this.gettotal();
                    }, deep: true, immediate: false
                }
            },
            computed: {

            },
            methods: {
                //更新试题分类
                updateparts: function (val) {
                    this.parts = val;
                    this.form.qpid = this.parts.map(obj => obj.Qp_ID).join(', ');
                },
                updateknl: function (val) {
                    this.knls = val;
                    this.form.knlid = this.knls.map(obj => obj.Qk_ID).join(', ');
                },
                //计算试题总数
                gettotal: function () {
                    var th = this;
                    if (th.loading_total || th.form.orgid == 0) return;
                    this.$refs['form'].validate((valid) => {
                        if (valid) {
                            th.loading_total = true;
                            $api.get('ExamQues/Total', th.form).then(req => {
                                if (req.data.success) {
                                    th.questotal = req.data.result;
                                } else {
                                    console.error(req.data.exception);
                                    throw req.config.way + ' ' + req.data.message;
                                }
                            }).catch(err => console.error(err))
                                .finally(() => th.loading_total = false);
                        } else th.questotal = 0;
                    });
                },
                //导出文件的按钮事件
                btnExportEvent: function () {
                    this.$refs['form'].validate((valid) => {
                        if (valid) {
                            if (this.loading_export) return;
                            if (this.questotal <= 1) {
                                alert("当前选择的试题数量为 0，无法导出");
                            } else if (this.questotal <= 1000) {
                                this.exportFile();
                            } else {
                                this.$confirm('试题数量 ' + this.questotal + ' 道, 导出时间会比较长，请耐心等待。点击确定继续', '提示', {
                                    confirmButtonText: '确定',
                                    cancelButtonText: '取消',
                                    type: 'warning'
                                }).then(() => {
                                    this.exportFile();
                                }).catch(() => { });
                            }
                        }
                    });
                },
                //生成导出文件
                exportFile: function () {
                    var th = this;
                    var form = $api.clone(th.form);
                    form['subpath'] = th.subpath;
                    th.loading_export = true;
                    $api.get('ExamQues/ExcelExport', form).then(function (req) {
                        if (req.data.success) {
                            let result = req.data.result;
                            console.log(result);
                            th.getFiles();
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => {
                        console.error(err);
                        alert(err);
                    }).finally(() => th.loading_export = false);
                },
                //获取文件列表
                getFiles: function () {
                    var th = this;
                    th.loading = true;
                    $api.get('ExamQues/ExcelFiles', { 'subpath': th.subpath, 'orgid': th.org.Org_ID }).then(function (req) {
                        if (req.data.success) {
                            th.files = req.data.result;
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err)).finally(() => th.loading = false);
                },
                //删除文件
                deleteFile: function (file) {
                    var th = this;
                    if (th.loading) return;
                    th.loading = true;
                    $api.delete('ExamQues/ExcelDelete', { 'orgid': th.org.Org_ID, 'filename': file, 'subpath': th.subpath }).then(function (req) {
                        if (req.data.success) {
                            th.getFiles();
                            th.$notify({
                                message: '文件删除成功！',
                                type: 'success', position: 'bottom-left', duration: 2000
                            });
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err)).finally(() => th.loading = false);
                },
                //删除所有文件
                deleteFileAll: function () {
                    var th = this;
                    if (th.loading) return;
                    th.loading = true;
                    $api.delete('ExamQues/ExcelDeleteAll', { 'orgid': th.org.Org_ID, 'subpath': th.subpath }).then(function (req) {
                        if (req.data.success) {
                            th.getFiles();
                            th.$notify({
                                message: '文件删除成功！',
                                type: 'success', position: 'bottom-right', duration: 2000
                            });
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err)).finally(() => th.loading = false);
                },
            },
        });

    });
