$ready(function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            id: $api.querystring('id'),  //试题分类的id，多个id用逗号隔开

            datas: [],      //所有数据,即试题关键字       
            form: { "orgid": "", "couid": "", "isdeleted": false, "name": "", "size": 999, "index": 1 },

            total: 0,       //当前机构下的试题关键字总数    
            questotal: 0,   //选中分类的试题总数    
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
                text: '清空', tips: '清空选择',
                id: 'clearn', type: 'warning',
                icon: 'e800'
            }]);
            var th = this;
            th.form.orgid = window.org.Org_ID;
            th.getDatas();
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
            //所取关键字的数据，为树形数据
            getDatas: function () {
                var th = this;
                th.loading = true;
                $api.get('ExamQues/TagPager', th.form).then(function (req) {
                    if (req.data.success) {
                        var result = req.data.result;
                        let selectedarr = th.id.split(',');
                        for (let i = 0; i < result.length; i++) {
                            result[i]['checked'] = false;
                            for (let j = 0; j < selectedarr.length; j++) {
                                if (result[i].Qtag_ID == selectedarr[j]) {
                                    result[i]['checked'] = true;
                                    break;
                                }
                            }
                        }
                        th.datas = result;
                        th.changeSelect(1);
                    } else {
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //获取选中关键字的试题总数
            getquestotal: function () {
                var th = this;
                return new Promise((resolve, reject) => {
                    th.loadstate.get = true;
                    let form = { "qtagid": "", "couid": 0, "qtype": -1, "use": true };
                    form.qtagid = th.selecteditems.map(p => p.Qtag_ID).join(',');
                    $api.get("ExamQues/TagQusTotal", form)
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
            //选中变更时
            changeSelect: function (d) {
                let arr = [];
                if (d != null) {
                    for (let i = 0; i < this.datas.length; i++) {
                        if (this.datas[i].checked) arr.push(this.datas[i]);
                    }
                } else {
                    for (let i = 0; i < this.datas.length; i++)
                        this.datas[i].checked = false;
                }

                this.selecteditems = arr;
                //像主窗体传值，传三个值：选中的分类，选中的试题数，调用函数名
                this.getquestotal().then(total => {
                    var pagebox = window.top.$pagebox;
                    if (pagebox && pagebox.source.top)
                        pagebox.source.box(window.name, 'vapp.receive', false, [arr, total, 'selecttag']);
                });
            }
        },
        filters: {

        },
        components: {

        }
    });
});