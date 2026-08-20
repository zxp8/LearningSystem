
$ready(function () {

    window.vapp = new Vue({
        el: '#vapp',
        data: {
            loading: false,  //
            id: $api.querystring('id'),
            defaultProps: {
                children: 'children',
                label: 'label'
            },
            datas: [] //数据源          
        },
        created: function () {
            var th = this;
            th.loading = true;
            //获取所有供选择的菜单项
            $api.get('ManageMenu/OrganMenus', { 'marker': 'organAdmin' }).then(function (req) {
                if (req.data.success) {
                    var result = req.data.result;
                    if (result != null && result.length > 0
                        && (result[0].children && result[0].children.length > 0)) {
                        th.datas = result[0].children;
                    }
                    console.error(th.datas);
                    //获取已经选择的菜单项
                    $api.get('ManageMenu/PositionPurview', { 'posid': th.id }).then(function (req) {
                        if (req.data.success) {
                            var arr = req.data.result;
                            for (var i = 0; i < arr.length; i++)
                                arr[i] = 'node_' + arr[i];
                            window.setTimeout(function () {
                                var trees = th.$refs.tree;
                                for (var i = 0; i < trees.length; i++)
                                    trees[i].setCheckedKeys(arr, true);
                                th.$nextTick(() => {
                                    //是否全部选中
                                    for (var i = 0; i < th.datas.length; i++) {
                                        th.isSelectedAll(i);
                                    }
                                });

                            }, 100);
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => alert(err))
                        .finally(() => th.loading = false);
                } else {
                    console.error(req.data.exception);
                    throw req.data.message;
                }
            }).catch(err => alert(err))
                .finally(() => { });
        },
        methods: {
            //确定保存
            btnEnter: function (isclose) {
                var th = this;
                if (th.loading) return;
                th.loading = true;
                //选中的菜单项
                var arr = new Array();
                var trees = th.$refs.tree;
                for (var i = 0; i < trees.length; i++) {
                    var nodes = trees[i].getCheckedNodes(true, false);
                    for (var j = 0; j < nodes.length; j++)
                        arr.push(nodes[j].MM_UID);
                }
                $api.post('ManageMenu/UpdatePositionPurview', { 'posid': th.id, 'mms': arr })
                    .then(function (req) {
                        if (req.data.success) {
                            var result = req.data.result;
                            th.$message({
                                type: 'success', center: true,
                                message: '操作成功!'
                            });
                            th.operateSuccess(isclose);
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => alert(err))
                    .finally(() => th.loading = false);
            },
            //是否全部选择
            isSelectedAll: function (index) {
                var tree = this.$refs.tree[index];
                var selectedNodes = tree.getCheckedNodes(false, true);
                // 递归获取所有节点的数量
                let getAllNodes = function (menunode) {
                    let total = 0;
                    if (menunode.children && menunode.children.length > 0) {
                        for (let i = 0; i < menunode.children.length; i++) {
                            total++;
                            total += getAllNodes(menunode.children[i]);
                        }
                    }
                    return total;
                };
                this.datas[index].MM_IsBold = getAllNodes(this.datas[index]) == selectedNodes.length;
                return this.datas[index].MM_IsBold;
            },
            //设置菜单文本样式
            setTextstyle: function (data) {
                let css = '';
                if (!$api.isnull(data.MM_Color) && data.MM_Color != '') css += 'color:' + data.MM_Color + ';';
                if (data.MM_IsBold) css += 'font-weight: bold;';
                if (data.MM_IsItalic) css += 'font-style: italic;';
                return css;
            },
            //设置图标样式
            setIcostyle: function (data) {
                let css = '';
                if (data.MM_IcoSize && data.MM_IcoSize != 0)
                    css += 'transform:' + 'scale(' + (1 + data.MM_IcoSize / 100) + ');';
                if (!$api.isnull(data.MM_IcoColor) && data.MM_IcoColor != '') css += 'color:' + data.MM_IcoColor + ';'
                css += 'margin-top:' + ($api.isnull(data.MM_IcoY) || data.MM_IcoY == 0 ? 0 : data.MM_IcoY) + 'px;';
                css += 'margin-left:' + ($api.isnull(data.MM_IcoX) || data.MM_IcoX == 0 ? 0 : data.MM_IcoX) + 'px;';
                //console.log(css);
                return css;
            },
            //全选或清空
            selected: function (root, index) {
                var arr = new Array();
                if (root.MM_IsBold) {
                    for (var i = 0; i < root.children.length; i++)
                        arr.push(root.children[i].id);
                }
                this.$refs.tree[index].setCheckedKeys(arr);
            },
            //操作成功
            operateSuccess: function (isclose) {
                //更新后触发的事件
                for (let i = 0; i < this.datas.length; i++) {
                    $api.cache('ManageMenu/OrganMenus:update', { 'marker': this.datas[i].MM_Marker });
                }
                if (window.top && window.top.$pagebox)
                    window.top.$pagebox.source.tab(window.name, 'vapp.getlist', isclose);
            }
        },
    });

});
