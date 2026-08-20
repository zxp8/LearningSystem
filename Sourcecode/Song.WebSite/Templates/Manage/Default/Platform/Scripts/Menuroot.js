$ready(function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            loading: false,
            loadingid: 0,        //当前操作中的对象id
            datas: [], //数据源           
            selects: [] //数据表中选中的行
        },
        watch: {
            'loading': function (val, old) {
                //console.log(val);
            }
        },
        created: function () {
            this.loadDatas();
        },
        methods: {
            //加载数据页
            loadDatas: function () {
                var th = this;
                th.loading = true;
                $api.get("ManageMenu/Root").then(function (d) {
                    if (d.data.success) {
                        th.datas = d.data.result;
                        $api.cache('ManageMenu/Menus:clear');
                    } else {
                        throw d.data.message;
                    }
                    th.rowdrop();
                }).catch(err => alert(err)).finally(() => th.loading = false);
            },
            //删除
            deleteData: function (datas) {
                var th = this;
                th.loading = true;
                $api.delete('ManageMenu/Delete', { 'id': datas }).then(function (req) {
                    if (req.data.success) {
                        var result = req.data.result;
                        th.$notify({
                            type: 'success',
                            message: '成功删除' + result + '条数据',
                            center: true
                        });
                        th.loadDatas();
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(err => alert(err)).finally(() => th.loading = false);
            },
            //更改使用状态
            changeUse: function (row) {
                var th = this;
                th.loadingid = row.MM_Id;
                $api.post('ManageMenu/Modify', { 'mm': row }).then(function (req) {
                    if (req.data.success) {
                        th.$notify({
                            type: 'success',
                            message: '修改状态成功!',
                            center: true
                        });
                        $api.cache('ManageMenu/Menus:clear');
                    } else {
                        throw req.data.message;
                    }
                }).catch(function (err) {
                    alert(err, '错误');
                }).finally(() => th.loadingid = 0);
            },
            //行的拖动
            rowdrop: function () {
                // 首先获取需要拖拽的dom节点            
                const el1 = document.querySelectorAll('table > tbody')[0];
                Sortable.create(el1, {
                    disabled: false, // 是否开启拖拽
                    ghostClass: 'sortable-ghost', //拖拽样式
                    handle: '.draghandle',     //拖拽的操作元素
                    animation: 150, // 拖拽延时，效果更好看 
                    onMove: function (evt, originalEvent) {
                        if ($dom('table tr.expanded').length > 0) return false;
                    },
                    onEnd: (e) => {
                        let arr = this.datas; // 获取表数据
                        arr.splice(e.newIndex, 0, arr.splice(e.oldIndex, 1)[0]); // 数据处理
                        this.$nextTick(function () {
                            this.datas = arr;
                            for (var i = 0; i < this.datas.length; i++) {
                                this.datas[i].MM_Order = i * 1;
                            }
                            this.changeTax();
                        });
                    }
                });
            },
            //更新排序
            changeTax: function () {
                var arr = $api.clone(this.datas);
                for (var i = 0; i < arr.length; i++)
                    delete arr[i]['childs'];
                var th = this;
                th.loading = true;
                $api.post('ManageMenu/ModifyTaxis', { 'items': arr }).then(function (req) {
                    if (req.data.success) {
                        th.$notify({
                            type: 'success',
                            message: '修改顺序成功!',
                            center: true
                        });
                    } else {
                        throw req.data.message;
                    }
                }).catch(err => alert(err)).finally(() => th.loading = false);
            }
        }
    });

});