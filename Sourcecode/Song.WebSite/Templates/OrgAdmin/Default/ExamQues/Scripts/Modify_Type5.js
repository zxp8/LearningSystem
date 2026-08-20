
$ready(['/Utilities/Components/question/function.js',
    '/Utilities/Scripts/marked.min.js', //markdown的处理，用于AI解析生成文件的处理
    '../Question/Components/ques_type.js',
    'Components/selectpart.js',
    'Components/modify_main.js',
    'Components/selectknl.js',
    'Components/general.js',
    'Components/ques_error.js',
    'Components/ques_wrong.js',
    'Components/ques_ansitem.js',
    'Components/ques_ansedit.js',
    'Components/enter_button.js'],
    function () {
        window.vapp = new Vue({
            el: '#vapp',
            data: {
                id: $api.dot(),
                org: {},           //当前机构
                config: {},      //当前机构配置项    
                types: [],        //试题类型，来自web.config中配置项

                entity: {},      //当前试题            
                ansitems: [],       //试题的选项
                ans_min: 3,          //选项最少几个              

                loading: false
            },
            watch: {
                'entity': {
                    handler: function (nv, ov) {
                        if (this.ansitems.length < 1 && $api.getType(this.entity.Qus_Items) == "Array") {
                            this.analysisitem();
                        }
                    }, immediate: false, deep: true
                }
            },
            updated: function () {
                this.$mathjax();
            },
            created: function () { },
            mounted: function () { },
            methods: {
                //创建新的选项
                newitem: async function () {
                    let obj = {
                        'Ans_ID': '',
                        'Qus_ID': 0,
                        'Qus_UID': '',
                        'Ans_Context': '',
                        'Ans_IsCorrect': false
                    };
                    let req = await $api.get('Snowflake/Generate');
                    if (req.data.success) {
                        let snowid = req.data.result;
                        obj.Ans_ID = snowid;
                    }
                    return obj;
                },
                //添加新的选项行
                additemrow: async function () {
                    let obj = await this.newitem();
                    this.$set(this.ansitems, this.ansitems?.length ?? 0, obj);
                },
                //解析选项的数据
                analysisitem: async function () {
                    this.ansitems = this.entity.Qus_Items;
                    while (this.ansitems.length < this.ans_min) {
                        let obj = await this.newitem();
                        this.$set(this.ansitems, this.ansitems.length, obj);
                    }
                    this.$nextTick(function () {
                        this.rowdrop();
                    });
                },
                //选项的编辑后的确认编辑
                ansEnter: function (ans) {
                    var index = this.ansitems.findIndex(x => x.Ans_ID == ans.Ans_ID);
                    var item = this.ansitems[index];
                    item.Ans_Context = ans.Ans_Context;
                },
                //行的拖动
                rowdrop: function () {
                    // 首先获取需要拖拽的dom节点            
                    const el1 = document.getElementById('ansitems');
                    Sortable.create(el1, {
                        disabled: false, // 是否开启拖拽
                        ghostClass: 'sortable-ghost', //拖拽样式
                        handle: '.text',     //拖拽的操作元素
                        animation: 150, // 拖拽延时，效果更好看
                        group: { // 是否开启跨表拖拽
                            pull: false,
                            put: false
                        },
                        onEnd: (e) => {
                            let arr = $api.clone(this.ansitems);
                            arr.splice(e.newIndex, 0, arr.splice(e.oldIndex, 1)[0]); // 数据处理
                            this.ansitems = [];
                            this.$nextTick(function () {
                                for (let i = 0; i < arr.length; i++)
                                    Vue.set(this.ansitems, i, arr[i]);
                                this.entity.Qus_Items = this.ansitems;
                            });
                        }
                    });
                },
                //更新排序
                changeTax: function () {
                    var arr = $api.clone(this.ansitems);
                    console.log(arr);
                },
                //验证方法
                verify: function (ques, alert) {
                    //将选项赋给试题
                    this.entity.Qus_Items = this.ansitems;
                    return true;
                },
                //试题加载完成
                quesload: function (ques) {
                    this.entity = ques;
                    this.analysisitem();
                    //重置题干的编辑框
                    let editor = this.$refs['editor_title'];
                    if (editor != null) editor.setContent(ques.Qus_Title);
                },
            },
        });
    }
);
