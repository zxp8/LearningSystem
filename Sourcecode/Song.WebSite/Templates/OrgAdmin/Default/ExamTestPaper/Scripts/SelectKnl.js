$ready(function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            id: $api.querystring('id'),  //试题知识点的id，多个id用逗号隔开

            datas: [],      //所有数据,即试题知识点       
            form: {
                orgid: '', search: '', isuse: true
            },
            defaultProps: {
                children: 'children',
                label: 'Qk_Name'
            },
            filterText: '',      //查询过虑树形的字符
            total: 0,       //当前机构下的试题知识点总数
            questotal: 0,   //选中分类的试题总数
            //是否折叠
            fold: false,
            showhelp: false, //是否显示帮助
            //选中的项
            selecteditems: [],

            loadstate: {
                init: false,        //初始化
                def: false,         //默认
                get: false,         //加载数据
                update: false,      //更新数据
                del: false          //删除数据
            }
        },
        mounted: function () {
            this.$refs.btngroup.addbtn([{
                text: '展开/折叠', tips: '展开树形或折叠树形',
                id: 'fold_open', type: 'primary',
                icon: 'e6ea'
            }, {
                text: '清空', tips: '清空选择',
                id: 'clearn', type: 'warning',
                icon: 'e800'
            }]);
            var th = this;
            th.form.orgid = window.org.Org_ID;
            th.getTreeData();
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
            //所取知识点的数据，为树形数据
            getTreeData: function () {
                var th = this;
                th.loading = true;
                $api.get('ExamQues/KnlTree', th.form).then(function (req) {
                    if (req.data.success) {
                        th.datas = th.clacCount(req.data.result);
                        th.$refs.tree.setCheckedKeys(th.id.split(','));
                        th.$nextTick(() => {
                            th.handleCheckChange();
                        });

                    } else {
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //获取选中知识点的试题总数
            getquestotal: function () {
                var th = this;
                return new Promise((resolve, reject) => {
                    th.loadstate.get = true;
                    let form = { "orgid": window.org.Org_ID, "qkid": "", "qtype": "", "use": true, "children": true };
                    form.qkid = th.selecteditems.map(p => p.Qk_ID).join(',');
                    $api.get("ExamQues/KnlQusTotal", form)
                        .then(req => {
                            if (req.data.success) {
                                th.questotal = req.data.result;
                                resolve(th.questotal);
                            } else {
                                console.error(req.data.exception);
                                throw req.config.way + ' ' + req.data.message;
                            }
                        }).catch(err => console.error(err))
                        .finally(() => th.loadstate.get = false);
                });
            },
            //计算课程数，ques数，
            clacCount: function (datas) {
                this.total = 0;
                this.calcSerial(datas);
                datas.forEach(d => this.ergodic_clacCount(d, 'Qk_Count', 'QuesCount'));
                return datas;
            },
            //遍历计算各个知识点的课程数，包括当前知识点的子知识点
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
                    childarr[i]['QuesCount'] = 0;
                    childarr[i]['calcChild'] = this.calcChild(childarr[i]);
                    this.total++;
                    this.calcSerial(childarr[i], childarr[i].serial);
                }
            },
            //计算子级节点的数量
            calcChild: function (data) {
                if (!data.children) return 0;
                let count = data.children.length;
                for (let i = 0; i < data.children.length; i++)
                    count += this.calcChild(data.children[i]);
                return count;
            },
            //过滤树形
            filterNode: function (value, data) {
                if (!value) return true;
                var txt = $api.trim(value.toLowerCase());
                if (txt == '') return true;
                return data.Qk_Name.toLowerCase().indexOf(txt) !== -1;
            },
            // 全部展开
            unFoldAll2: function (data) {
                let self = this;
                data.forEach((el) => {
                    self.$refs.tree.store.nodesMap[el.Qk_ID].expanded = true;
                    el.children && el.children.length > 0 ? self.unFoldAll2(el.children) : ""; // 子级递归
                });
            },
            // 全部折叠
            collapseAll2: function (data) {
                let self = this;
                data.forEach((el) => {
                    self.$refs.tree.store.nodesMap[el.Qk_ID].expanded = false;
                    el.children && el.children.length > 0 ? self.collapseAll2(el.children) : ""; // 子级递归
                });
            },
            //选择变更
            handleCheckChange: function (data, checked, indeterminate) {
                //当前选中的数据
                //如果某个节点下的下级节点全部选中了，则只取当前节点；只有是半选，返回选中的子节点，且不包括自身（半选中）的节点。
                let nodes = this.getProcessedCheckedKeys();
                this.selecteditems = nodes;
                //像主窗体传值，传三个值：选中的分类，选中的试题数，调用函数名
                this.getquestotal().then(total => {
                    var pagebox = window.top.$pagebox;
                    if (pagebox && pagebox.source.top)
                        pagebox.source.box(window.name, 'vapp.receive', false, [nodes, total, 'selectknl']);
                });
            },
            // 获取处理后的选中节点ID数组
            getProcessedCheckedKeys: function () {
                const treeStore = this.$refs.tree.store;
                const resultNodes = [];
                // 递归遍历函数
                const traverse = (node) => {
                    // 获取当前节点的直接子节点列表
                    const childNodes = node.root ? node.root.childNodes : node.childNodes;
                    childNodes.forEach(child => {
                        if (child.checked) resultNodes.push(child.data);    // 情况1: 节点被全选
                        else if (child.indeterminate) traverse(child);    // 情况2: 节点处于半选状态
                    });
                };
                // 从根store开始遍历
                traverse(treeStore);
                return resultNodes;
            }
        },
        filters: {

        },
        components: {
            'quescount': {
                props: ['id'],
                data: function () {
                    return {
                        total: 0,
                        loading: false,
                    }
                },
                watch: {
                    'id': {
                        handler: function (nv, ov) {
                            this.getquestotal(nv).then(total => {                               
                                this.total = total;
                            });
                        }, immediate: true,
                    }
                },
                methods: {
                    //获取选中分类的试题总数
                    getquestotal: function () {
                        var th = this;
                        return new Promise((resolve, reject) => {
                            th.loading = true;
                            let form = { "orgid": window.org.Org_ID, "qkid": th.id, "qtype": "", "use": true, "children": true };                      
                            $api.get("ExamQues/KnlQusTotal", form)
                                .then(req => {
                                    if (req.data.success) {                                      
                                        resolve(req.data.result);
                                    } else {
                                        console.error(req.data.exception);
                                        throw req.config.way + ' ' + req.data.message;
                                    }
                                }).catch(err => console.error(err))
                                .finally(() => th.loading = false);
                        });
                    },
                },
                template: `<template>
                    <loading asterisk v-if="loading"></loading>
                    <span v-else class="qcount" title="试题数" question :zero="total<=0">{{total|commas}}道</span>
                </template>`
            }
        }
    });
});