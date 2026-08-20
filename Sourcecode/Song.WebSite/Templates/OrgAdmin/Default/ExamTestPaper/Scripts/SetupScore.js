$ready(['../Question/Components/ques_type.js',
    'Components/quesscores.js'   //试题的分数
],
    function () {
        window.vapp = new Vue({
            el: '#vapp',
            data: {
                types: [],       //题型
                //试卷对象  
                entity: {},
                //试题的类型数据，例如题型，该题型的题量，分数，分数占比，
                qtypeitems: [],
                scoreitems: [],      //用于分数计算，是qtypeitems的拷贝
                //录入校验的规划
                rules: {
                    Etp_Total: [
                        { required: true, message: '分数不得为空', trigger: 'blur' },
                        {
                            validator: function (rule, value, callback) {
                                if (!(/^[1-9]\d*$/.test(value))) return callback(new Error('请输入大于零的整数'));
                                if (Number(value) < Number(vapp.entity.Etp_PassScore))
                                    return callback(new Error('试卷总分不得小于及格分'));;
                                callback();
                            }, trigger: 'blur'
                        }
                    ],
                    Etp_PassScore: [
                        { required: true, message: '分数不得为空', trigger: 'blur' },
                        {
                            validator: function (rule, value, callback) {
                                if (!(/^[1-9]\d*$/.test(value))) return callback(new Error('请输入大于零的整数'));
                                if (Number(value) > vapp.entity.Etp_Total) callback(new Error('及格分不得大于满分'));
                                else callback();

                            }, trigger: 'blur'
                        }
                    ],
                    total: [
                        {
                            validator: function (rule, value, callback) {
                                let items = vapp.qtypeitems;
                                let percent = items.reduce((a, b) => a + b.percent, 0);
                                if (percent > 100) return callback(new Error('各题型分数占比之和不能大于100'));
                                if (percent != 100) return callback(new Error('各题型分数占比之和必须等于100'));

                                for (let i = 0; i < items.length; i++) {
                                    const el = items[i];
                                    if (el.count > el.total) {
                                        return callback(new Error('' + el.name + '题的数量不能大于' + el.total + '道'));
                                    }
                                    if (el.count <= 0 && el.percent > 0) {
                                        return callback(new Error('' + el.name + '题的数量不能小于1道'));
                                    }
                                    if (el.percent <= 0 && el.count > 0) {
                                        return callback(new Error('' + el.name + '题的分数不可小于1分'));
                                    }
                                }
                                callback();
                            }, trigger: 'blur'
                        }
                    ],

                },
                loadstate: {
                    init: false,        //初始化
                    def: false,         //默认
                    get: false,         //加载数据
                    update: false,      //更新数据
                    del: false          //删除数据
                }
            },
            mounted: function () {
                var th = this;
                this.receive().then(qtypeitems => {
                    //添加题型录入的验证方法
                    for (var i = 0; i < qtypeitems.length; i++) {
                        const item = qtypeitems[i];
                        //题型数量的录入验证
                        let rulecount = [{
                            validator: function (rule, value, callback) {
                                let field = rule.field;
                                let type = Number(field.substring(field.length - 1));   //题型
                                let item = vapp.qtypeitems.find(i => i.type == type);
                                if (item.count < 0) return callback(new Error('请输入大于零的整数'));
                                //试题数不得大于可选数
                                if (item.count > item.total) return callback(new Error("试题数不得大于可选数"));
                                //分数占比大于零，试题不得为零
                                if (item.percent > 0 && item.count == 0) return callback(new Error("试题数不可为零"));
                                callback();
                            }, trigger: 'blur'
                        }];
                        th.$set(th.rules, 'count' + item.type, rulecount);
                        //题型分数点比的验证
                        let rulepercent = [{
                            validator: function (rule, value, callback) {
                                let field = rule.field;
                                let type = Number(field.substring(field.length - 1));   //题型
                                let item = vapp.qtypeitems.find(i => i.type == type);
                                if (item.percent < 0) return callback(new Error('请输入大于零的整数'));
                                //分数占比大于零，试题不得为零
                                if (item.percent == 0 && item.count > 0) return callback(new Error("不可为零"));
                                //分数占比大于零，试题不得为零
                                let total = vapp.qtypeitems.reduce((a, b) => a + b.percent, 0);
                                if (total != 100 && item.percent != 0) return callback(new Error("各题型的占比之和必须为100"));
                                callback();

                            }, trigger: 'blur'
                        }];
                        th.$set(th.rules, 'percent' + item.type, rulepercent);
                    }
                });
                this.$nextTick(function () {
                    //初始化题型的排序方法
                    vapp.rowdrop();
                });


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
                },
                //各题型占比总和
                percenttotal: function () {
                    return this.qtypeitems.reduce((a, b) => a + b.percent, 0);
                }
            },
            watch: {

            },
            methods: {
                //向“更多分数设置”的主窗体传递数据
                transmit: function (entity, qtypeitems) {
                    //像主窗体传值，传三个值：选中的分类，选中的试题数，调用函数名
                    var pagebox = window.top.$pagebox;
                    if (pagebox && pagebox.source.top) {
                        //试卷对象，题型分数分配的数据
                        pagebox.source.box(window.name, 'vapp.scorereceive', false, [entity, qtypeitems]);
                    }
                },
                //接收“更多分数设置”的主窗体数据
                receive: function () {
                    return new Promise((resolve, reject) => {
                        //像主窗体传值，传三个值：选中的分类，选中的试题数，调用函数名
                        var pagebox = window.top.$pagebox;
                        if (pagebox && pagebox.source.top) {
                            [this.entity, this.types, this.qtypeitems] = pagebox.source.box(window.name, 'vapp.scoretransmit', false);
                            this.scoreitems = this.qtypeitems;
                            resolve(this.qtypeitems);
                        }
                    });
                },
                //当试卷总分更改时
                chanageTotal: function () {
                    let tptotal = this.entity.Etp_Total;
                    let tmscore = 0;
                    this.qtypeitems.forEach(el => {
                        el.number = Math.floor(el.percent * tptotal / 100);
                        tmscore += el.number;
                    });
                    //重新计算各题型占比的总和
                    let percenttotal = this.qtypeitems.reduce((a, b) => a + b.percent, 0);
                    if (percenttotal == 100) {
                        if (tmscore - tptotal > 0) {
                            const maxel = this.qtypeitems.reduce((p, c) => p.number > c.number ? p : c);
                            this.$set(maxel, 'number', maxel.number - (tmscore - tptotal));
                        } else {
                            const minxel = this.qtypeitems.reduce((p, c) => p.number < c.number ? p : c);
                            this.$set(minxel, 'number', minxel.number - (tmscore - tptotal));
                        }
                    }
                    this.$refs['form'].validate();
                },
                //当题型的试题量变化时
                changeCount: function (e) {
                    this.$refs['form'].validate();
                },
                //行的拖动
                rowdrop: function () {
                    // 首先获取需要拖拽的dom节点            
                    const el1 = document.querySelectorAll('div.qtype_card .el-card__body')[0];
                    if (el1 == null) return;
                    Sortable.create(el1, {
                        disabled: false, // 是否开启拖拽
                        ghostClass: 'sortable-ghost', //拖拽样式
                        handle: '.draghandle',     //拖拽的操作元素
                        animation: 150, // 拖拽延时，效果更好看
                        group: { pull: false, put: false },
                        onEnd: (e) => {
                            let typerow = $dom(".qtypeitems");
                            var th = this;
                            var arr = [];
                            typerow.each(function () {
                                let type = Number($dom(this).attr('type'));
                                let item = th.qtypeitems.find(el => Number(el.type) == type);
                                arr.push(item);
                            });
                            th.$nextTick(function () {
                                th.rowdrop();
                                th.transmit(th.entity, arr);

                            });
                            th.scoreitems = [];
                            th.scoreitems = arr;
                        }
                    });
                },
            },
            filters: {

            },
            components: {
                //试题题型的分数
                'scores': {
                    props: ['count', 'number'],
                    data: function () {
                        return {
                            list: []
                        }
                    },
                    computed: {
                        'data': t => [t.count, t.number]
                    },
                    watch: {
                        'data': {
                            handler: function (val) {
                                let [count, number] = val;
                                this.list = [];
                                if (count == 0) this.list = [];
                                else {
                                    let num = Math.floor(number / count * 100) / 100;
                                    let tmtotal = 0;  //题型总分，计算所得
                                    for (let i = 0; i < count; i++) {
                                        tmtotal += num;
                                        this.list.push(num);
                                    }
                                    if (tmtotal != number) {
                                        let last = this.list[this.list.length - 1];
                                        this.list[this.list.length - 1] = Math.floor((last + number - tmtotal) * 100) / 100;
                                    }
                                }
                            }, immediate: true,
                        }
                    },
                    methods: {},
                    template: `<div class="scores">
                <div v-for="(item,idx) in list" :zero="item<=0">
                    <span>{{idx+1}}</span>
                    <span :zero="item<=0"><b>{{item}}</b> 分</span>
                </div>
            </div>`
                }
            }
        });
    });