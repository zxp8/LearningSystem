$ready(function () {

    window.vapp = new Vue({
        el: '#vapp',
        data: {
            datas: [],      //所有数据
            form: {
                orgid: '', search: '', isuse: null
            },
            defaultProps: {
                children: 'children',
                label: 'Qp_Name'
            },
            expanded: [],        //树形默认展开的节点
            expanded_storage: 'quespart_for_admin_tree',  //用于记录展开节点的storage名称
            filterText: '',      //查询过虑树形的字符
            total: 0,       //当前机构下的试题分类总数
            //是否折叠
            fold: true,

            loading: true,
            loadingid: -1,
            loading_sumbit: false,   //提交时的预载           
            loading_init: true
        },
        mounted: function () {
            this.$refs.btngroup.addbtn({
                text: '更新数据', tips: '更新课程数、试题数、试卷数的统计数据',
                id: 'update_data', type: 'warning',
                icon: 'e651'
            });
            this.$refs.btngroup.addbtn({
                text: '展开/折叠', tips: '展开树形或折叠树形',
                id: 'fold_open', type: 'primary',
                icon: 'e6ea'
            });
        },
        created: function () {
            //console.error(window.org);
            //console.error(window.config);

            var th = this;
            th.form.orgid = window.org.Org_ID;
            th.getTreeData();
        },
        computed: {
        },
        watch: {
            //过滤树形数据
            filterText: function (val) {
                this.$refs.tree.filter(val);
                this.fold = true;
            },
            //树形是否折叠
            fold: function (nv, ov) {
                if (nv) this.unFoldAll2(this.datas);
                else this.collapseAll2(this.datas);
            }
        },
        methods: {
            //所取专业的数据，为树形数据
            getTreeData: function () {
                var th = this;
                th.loading = true;
                $api.get('ExamQues/PartTree', th.form).then(function (req) {
                    if (req.data.success) {
                        th.datas = th.clacCount(req.data.result);
                        //获取默认展开的节点
                        var arr = $api.storage(th.expanded_storage);
                        if ($api.getType(arr) == 'Array') {
                            th.expanded = arr;
                        }
                    } else {
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //计算课程数，ques数，test数
            clacCount: function (datas) {
                this.total = 0;
                this.calcSerial(datas);
                datas.forEach(d => this.ergodic_clacCount(d, 'QP_Count', 'QuesCount'));
                return datas;
            },
            //遍历计算各个专业的课程数，包括当前专业的子专业
            //field:要计算的字段
            //result:计算结果的字段名，主要为了保留field原始值，方便恢复
            ergodic_clacCount: function (sbj, field, result) {
                let count = sbj[field];
                if (sbj.children && sbj.children.length > 0) {
                    let datas = sbj.children;
                    for (let i = 0; i < datas.length; i++)
                        count += this.ergodic_clacCount(datas[i], field, result);
                }
                sbj[result] = count;
                return count;
            },
            //计算序号
            calcSerial: function (item, lvl) {
                var childarr = Array.isArray(item) ? item : (item.children ? item.children : null);
                if (childarr == null) return;
                for (let i = 0; i < childarr.length; i++) {
                    childarr[i].serial = (lvl ? lvl : '') + (i + 1) + '.';
                    //childarr[i]['CourseCount'] = 0;
                    childarr[i]['QuesCount'] = 0;
                    //childarr[i]['TestCount'] = 0;
                    childarr[i]['calcChild'] = this.calcChild(childarr[i]);
                    this.total++;
                    this.calcSerial(childarr[i], childarr[i].serial);
                }
            },
            calcChild: function (sbj) {
                if (!sbj.children) return 0;
                var count = sbj.children.length;
                for (var i = 0; i < sbj.children.length; i++) {
                    count += this.calcChild(sbj.children[i]);
                }
                return count;
            },
            //拖动改变顺序
            handleDragEnd(draggingNode, dropNode, dropType, ev) {
                var th = this;
                th.loading_sumbit = true;
                var arr = th.tree2array(this.datas);
                $api.post('ExamQues/PartModifyTaxis', { 'list': arr }).then(function (req) {
                    if (req.data.success) {
                        var result = req.data.result;
                        th.$message({
                            type: 'success',
                            message: '更改排序成功!',
                            center: true
                        });
                        th.fresh_cache();
                        th.clacCount(th.datas);
                        //th.getTreeData();
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading_sumbit = false);
            },

            //节点展开事件
            nodeexpand: function (data, node, tree) {
                this.expanded.push(data.Qp_ID);
                $api.storage(this.expanded_storage, this.expanded);
            },
            //节点折叠事件
            nodecollapse: function (data, node, tree) {
                var index = this.expanded.indexOf(data.Qp_ID);
                if (index > -1) {
                    this.expanded.splice(index, 1);
                    $api.storage(this.expanded_storage, this.expanded);
                }
            },
            // 全部展开
            unFoldAll2: function (data) {
                let self = this;
                data.forEach((el) => {
                    self.$refs.tree.store.nodesMap[el.Qp_ID].expanded = true;
                    el.children && el.children.length > 0 ? self.unFoldAll2(el.children) : ""; // 子级递归
                });
            },
            // 全部折叠
            collapseAll2: function (data) {
                let self = this;
                data.forEach((el) => {
                    self.$refs.tree.store.nodesMap[el.Qp_ID].expanded = false;
                    el.children && el.children.length > 0 ? self.collapseAll2(el.children) : ""; // 子级递归
                });
            },
            //过滤树形
            filterNode: function (value, data) {
                if (!value) return true;
                var txt = $api.trim(value.toLowerCase());
                if (txt == '') return true;
                return data.Qp_Name.toLowerCase().indexOf(txt) !== -1;
            },
            //编辑当前专业
            modify: function (row) {
                this.$refs.btngroup.modify(row[this.$refs.btngroup.idkey]);
            },
            //修改状态
            changeState: function (data, field) {
                data[field] = !data[field];
                var th = this;
                this.loadingid = data.Qp_ID;
                $api.post('ExamQues/PartModify', { 'entity': data }).then(function (req) {
                    th.loadingid = -1;
                    if (req.data.success) {
                        th.$message({
                            type: 'success',
                            message: '修改状态成功!',
                            center: true
                        });
                        th.fresh_cache();
                        $api.cache('ExamQues/PartForID:clear', { 'id': data.Qp_ID });
                    } else {
                        throw req.data.message;
                    }
                }).catch(function (err) {
                    alert(err, '错误');
                    th.loadingid = -1;
                });
            },
            //将树形数据转到数据列表，用于递交到服务端更改专业的排序
            tree2array: function (datas) {
                var list = [];
                list = toarray(datas, 0, 1, list);
                return list;
                function toarray(arr, pid, level, list) {
                    for (let i = 0; i < arr.length; i++) {
                        const d = arr[i];
                        var obj = {
                            'Qp_ID': d.Qp_ID,
                            'Qp_PID': pid,
                            'Qp_Order': i + 1,
                        }
                        list.push(obj);
                        if (d.children && d.children.length > 0)
                            list = toarray(d.children, d.Qp_ID, ++level, list);
                    }
                    return list;
                }
            },
            //删除节点
            remove: function (node, data) {
                if (data.children && data.children.length > 0) {
                    var msg = '当前分类下还有子分类，将一并删除';
                    this.$confirm(msg, '提示', {
                        dangerouslyUseHTMLString: true,
                        confirmButtonText: '确定',
                        cancelButtonText: '取消',
                        type: 'warning'
                    }).then(() => {
                        this.remove_func(node, data)
                    }).catch(() => { });
                } else this.remove_func(node, data);

            },
            //删除分类的具体方法
            remove_func: function (node, data) {
                var th = this;
                th.loading_sumbit = true;
                $api.delete('ExamQues/PartDelete', { 'id': data.Qp_ID }).then(function (req) {
                    if (req.data.success) {
                        var result = req.data.result;
                        th.$message({
                            type: 'success',
                            message: '删除成功!',
                            center: true
                        });
                        th.fresh_cache();
                        th.getTreeData();
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                }).finally(() => th.loading_sumbit = false);
            },
            //当专业数据更改时，刷新缓存数据
            fresh_cache: function () {
                $api.cache('ExamQues/PartTreeFront:update', { 'orgid': window.org.Org_ID }, this.getTreeData());
            },
            //更新统计数据，包括课程数、试题数、试卷数
            update_statdata: function () {
                this.$confirm('此操作将重新计算试题分类下的试题数，, 是否继续?', '更新数据', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'warning'
                }).then(() => {
                    var th = this;
                    var loading = this.$fulloading();
                    $api.post('ExamQues/updatestatisticaldata', { 'orgid': window.org.Org_ID, 'sbjid': '' }).then(req => {
                        if (req.data.success) {
                            var result = req.data.result;
                            th.getTreeData();
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => {
                            th.$nextTick(function () {
                                loading.close();
                            });
                        });
                }).catch(() => { });
            }

        },
        filters: {
            //数字转三位带逗号
            'commas': function (number) {
                return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }
        }
    });
});
