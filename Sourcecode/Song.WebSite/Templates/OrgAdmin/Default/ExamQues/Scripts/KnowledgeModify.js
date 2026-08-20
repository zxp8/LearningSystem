
$ready(function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            id: $api.querystring('id'),
            activeName: 'general',      //选项卡
            datas: [],     //所有知识点
            //当前要操作的对象
            entity: {
                Qk_ID: 0,
                Qk_IsUse: true,
                Qk_PID: '',
                Org_ID: window.org.Org_ID,
                Qk_Order: 0
            },
            //专业树形下拉选择器的配置项
            defaultProps: {
                children: 'children',
                label: 'Qk_Name',
                value: 'Qk_ID',
                disabled: 'Qk_IsUse',
                expandTrigger: 'hover',
                checkStrictly: true
            },
            selects: [],      //选择中的项
            rules: {
                Qk_Name: [{ required: true, message: '不得为空', trigger: 'blur' },
                { validator: validate.name.proh, trigger: 'change' },   //禁止使用特殊字符
                { validator: validate.name.danger, trigger: 'change' }
                ]
            },
            loading: false,
            loading_init: true,      //初始化预载
            num: 0
        },
        computed: {
            //是否新增账号
            'isadd': t => t.id == null || t.id == '' || this.id == 0,
        },
        watch: {
        },
        created: function () {
            this.getTreeData(window.org.Org_ID);
        },
        mounted: function () {

        },
        methods: {
            //获取课程专业的数据
            getTreeData: function (orgid) {
                var th = this;
                this.loading = true;
                $api.get('ExamQues/KnlTree', { orgid: orgid, search: '', isuse: null })
                    .then(function (req) {
                        if (req.data.success) {
                            let datas = req.data.result;
                            th.calcSerial(datas);
                            th.datas = datas;
                        } else {
                            throw req.data.message;
                        }
                    }).catch(function (err) {
                        console.error(err);
                    }).finally(function () {
                        th.loading = false;
                        th.getEntity();
                    });
            },
            //计算序号
            calcSerial: function (item, lvl) {
                var childarr = Array.isArray(item) ? item : (item.children ? item.children : null);
                if (childarr == null) return;
                for (let i = 0; i < childarr.length; i++) {
                    childarr[i].serial = (lvl ? lvl : '') + (i + 1) + '.';
                    this.calcSerial(childarr[i], childarr[i].serial);
                }
            },
            //获取当前实体
            getEntity: function () {
                var th = this;
                th.loading = true;
                if (th.id == '' || th.id == null) {
                    $api.get('Snowflake/Generate').then(function (req) {
                        if (req.data.success) {
                            th.entity.Qk_ID = req.data.result;
                            th.traversalUse(th.datas);
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
                }
                $api.get('ExamQues/KnlForID', { 'id': th.id }).then(function (req) {
                    if (req.data.success) {
                        var result = req.data.result;
                        th.entity = result;
                        //设置当前节点禁用，防止选择自身
                        var sbj = th.traversalQuery(result.Qk_ID, th.datas);
                        if (sbj != null) sbj.Qk_IsUse = false;
                        th.traversalUse(th.datas);
                        //将当前专业的上级专业，在控件中显示
                        var arr = [];
                        arr = th.getParentPath(th.entity, th.datas, arr);
                        th.selects = arr;
                    } else {
                        throw '未查询到数据';
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            btnEnter: function (formName, isclose) {
                if (!isclose && this.isadd) return;
                var th = this;
                this.$refs[formName].validate((valid, fields) => {
                    if (valid) {
                        if (th.loading) return;
                        let sbj = th.clone(th.entity);
                        th.loading = true;
                        //接口路径
                        let apipath = th.id == '' ? 'ExamQues/KnlAdd' : 'ExamQues/KnlModify';
                        //接口参数，如果有上传文件，则增加file
                        let para = { 'entity': sbj };
                        $api.post(apipath, para).then(function (req) {
                            th.loading = false;
                            if (req.data.success) {
                                var result = req.data.result;
                                th.$notify({
                                    type: 'success', position: 'bottom-left',
                                    message: isclose ? '保存成功，并关闭！' : '保存当前编辑成功！'
                                });
                                th.operateSuccess(isclose);
                            } else {
                                throw req.data.message;
                            }
                        }).catch(err => alert(err, '错误'))
                            .finally(() => th.loading = false);
                    } else {
                        //未通过验证的字段
                        let field = Object.keys(fields)[0];
                        let label = $dom('label[for="' + field + '"]');
                        while (label.attr('tab') == null)
                            label = label.parent();
                        th.activeName = label.attr('tab');
                        console.log('error submit!!');
                        return false;
                    }
                });
            },
            //为上传数据作处理
            clone: function (entity) {
                var obj = $api.clone(entity);
                //上级专业
                if (this.selects.length > 0)
                    obj.Qk_PID = this.selects[this.selects.length - 1];
                else
                    obj.Qk_PID = 0;
                if (obj.Qk_PID == '') obj.Qk_PID = 0;

                return obj;
            },
            //操作成功
            operateSuccess: function (isclose) {
                $api.cache('Subject/ForID:clear', { 'id': this.id });
                window.top.$pagebox.source.tab(window.name, 'vapp.getTreeData', isclose);
            },
            //获取当前专业的上级路径
            getParentPath: function (entity, datas, arr) {

                var obj = this.traversalQuery(entity.Qk_PID, datas);
                if (obj == null) return arr;
                arr.splice(0, 0, obj.Qk_ID);
                arr = this.getParentPath(obj, datas, arr);
                return arr;
            },
            //从树中遍历对象
            traversalQuery: function (sbjid, datas) {
                var obj = null;
                for (let i = 0; i < datas.length; i++) {
                    const d = datas[i];
                    if (d.Qk_ID == sbjid) {
                        obj = d;
                        break;
                    }
                    if (d.children && d.children.length > 0) {
                        obj = this.traversalQuery(sbjid, d.children);
                        if (obj != null) break;
                    }
                }
                return obj;
            },
            //所有Qk_IsUse取反，主要是Cascader控件的disabled取了Qk_IsUse作为值，它俩的意义相反
            traversalUse: function (datas, use) {
                for (let i = 0; i < datas.length; i++) {
                    const d = datas[i];
                    d.Qk_IsUse = use == null ? !d.Qk_IsUse : use;

                    if (d.children && d.children.length > 0)
                        obj = this.traversalUse(d.children, d.Qk_IsUse ? true : null);
                }
            }
        },
    });

});
