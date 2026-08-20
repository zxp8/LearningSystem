$ready([
    '/Utilities/Components/question/function.js',
    '../Question/Components/ques_type.js',
    'Components/ques_diff.js',
    'Components/ques_collect.js',
    'Components/selecttag.js',
], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            types: [],        //试题类型，来自web.config中配置项
            admin: {},          //当前登录用户
            //试题的查询条件
            form: {"acid":"","search":"","qpid":"","tagid":"","knlid":"","type":"","diff":"","use":true,"error":"","wrong":"","size":10,"index":1},
            datas: [],
            total: 1, //总记录数
            totalpages: 1, //总页数
            selects: [], //数据表中选中的行

            loadstate: {
                init: false,        //初始化
                def: false,         //默认
                get: false,         //加载数据
                update: false,      //更新数据
                del: false          //删除数据
            },
            loadingid: 0,
        },
        updated: function () {
            this.$mathjax();
        },
        mounted: function () {
            this.$refs.btngroup.addbtn([{
                text: '全选/取消', tips: '选择标签',
                id: 'select', type: 'primary',
                icon: 'a057'
            }, {
                text: '取消收藏', tips: '取消收藏',
                id: 'cancel', type: 'warning',
                icon: 'e747'
            }]);

            var th = this;
            th.loadstate.init = true;
            $api.cache('Question/Types:99999').then(req => {
                if (req.data.success) {
                    th.types = req.data.result;
                } else {
                    throw req.data.message;
                }
            }).catch(err => console.error(err))
                .finally(() => th.loadstate.init = false);

            //当前登录的管理员
            $api.login.current('admin', d => {
                th.admin = d;
                th.form.acid = d.Acc_Id;
                //重新设置查询面板的初始参数
                th.$refs['query_panel'].setmodel(th.form);
                th.handleCurrentChange(1);
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
            }
        },
        watch: {

        },
        methods: {
            //加载数据页
            handleCurrentChange: function (index) {
                var th = this;
                if (index != null) this.form.index = index;
                var loading = this.$fulloading();            
                th.loadstate.get = true;
                $api.get("ExamQues/CollectPager", th.form).then(function (d) {
                    if (d.data.success) {
                        var result = d.data.result;
                        for (let i = 0; i < result.length; i++) {
                            result[i] = window.ques.parseAnswer(result[i]);
                            result[i]["checked"] = false;
                            result[i]["showdetail"] = false;
                            //result[i].Qus_Title = result[i].Qus_Title.replace(/(<([^>]+)>)/ig, "");
                        }
                        th.datas = result;
                        th.totalpages = Number(d.data.totalpages);
                        th.total = d.data.total;
                    } else {
                        throw d.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                }).finally(() => {                  
                    th.$nextTick(function () {
                        loading.close();
                        th.loadstate.get = false;
                    });
                });
            },
            //查询面板重置的方法
            queryreset: function () {
                this.$refs['selecttag'].clear();
            },
            //批量选择
            selectall: function () {
                let isselected = true;   //是否全选
                for (let i = 0; i < this.datas.length; i++) {
                    if (this.datas[i].checked == null || !this.datas[i].checked) {
                        isselected = false;
                        break;
                    }
                }
                for (let i = 0; i < this.datas.length; i++) {
                    this.$set(this.datas[i], 'checked', !isselected);
                }
            },
            //取消收藏
            cancelselected: function () {
                var arr = [];
                for (let i = 0; i < this.datas.length; i++)
                    if (this.datas[i].checked)
                        arr.push(this.datas[i].Qus_ID);
                if (arr.length < 1) {
                    this.$message({
                        message: '请选中要操作的数据项',
                        type: 'error'
                    });
                    return false;
                }
                //console.error(arr);

                var th = this;
                th.loadstate.update = true;
                $api.delete("ExamQues/CollectRemove", { "accid": th.form.acid, "qusid": arr.join(",") })
                    .then(req => {
                        if (req.data.success) {
                            let result = req.data.result;
                            th.$message({
                                message: '成功取消' + result + '条收藏信息',
                                type: 'success'
                            });
                            th.handleCurrentChange();
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                    .finally(() => th.loadstate.update = false);
            },

        },
        filters: {

        },
        components: {
            //试题的关键字
            'tags': {
                props: ['ques'],
                data: function () {
                    return {
                        taglist: null,  //关键字列表
                        loading: false,
                    }
                },
                watch: {
                    "ques": {
                        handler: function (val) {
                            this.gettags();
                        }, immediate: true,
                    }
                },
                methods: {
                    //获取关键字
                    gettags: function () {
                        var th = this;
                        if (th.loading) return;
                        th.loading = true;
                        $api.get("ExamQues/TagForQues", { "quesid": th.ques.Qus_ID })
                            .then(req => {
                                if (req.data.success) {
                                    th.taglist = req.data.result;
                                } else {
                                    console.error(req.data.exception);
                                    throw req.config.way + ' ' + req.data.message;
                                }
                            }).catch(err => console.error(err))
                            .finally(() => th.loading = false);
                    }
                },
                template: `<div>
                <loading v-if="loading"></loading>
                <el-tag v-else-if="taglist==null || taglist.length<1" type="info">没有关键字</el-tag>
                <el-tag v-else v-for="tag in taglist" :key="tag.Tag_ID" type="warning" >
                    <el-tooltip :content="'关键字  '+tag.Qtag_Name" placement="bottom" effect="light">
                        <span>{{tag.Qtag_Name}}</span>
                    </el-tooltip>
                </el-tag>
            </div>`
            },
            //试题的分类
            'parts': {
                props: ['ques'],
                data: function () {
                    return {
                        parts: null,  //分类列表
                        loading: false,
                    }
                },
                watch: {
                    "ques": {
                        handler: function (val) {
                            this.getparts();
                        }, immediate: true,
                    }
                },
                methods: {
                    //获取关联的分类
                    getparts: function () {
                        var th = this;
                        if (th.loading) return;
                        th.loading = true;
                        $api.get("ExamQues/PartForQues", { "qusid": th.ques.Qus_ID })
                            .then(req => {
                                if (req.data.success) {
                                    th.parts = req.data.result;
                                } else {
                                    console.error(req.data.exception);
                                    throw req.config.way + ' ' + req.data.message;
                                }
                            }).catch(err => console.error(err))
                            .finally(() => th.loading = false);
                    }
                },
                template: `<div class="parts">
                    <loading v-if="loading"></loading>                   
                    <el-tag v-else v-for="p in parts" >
                        <el-tooltip :content="'试题分类：  '+p.Qp_Name" placement="bottom" effect="light">
                            <span>{{p.Qp_Name}}</span>
                        </el-tooltip>
                    </el-tag>
                </div>`
            },
            //试题的答案
            'answer': {
                props: ['ques'],
                methods: {
                    //解析试题答案
                    answer: function (q) {
                        let func = 'type' + q.Qus_Type;
                        return this[func] != null ? this[func](q) : '';
                    },
                    type1: q => String.fromCharCode(65 + q.Qus_Items.findIndex(m => m.Ans_IsCorrect)),
                    type2: q => q.Qus_Items.map((m, i) => m.Ans_IsCorrect ? String.fromCharCode(65 + i) : null)
                        .filter(m => m !== null).join('、'),
                    type3: q => q.Qus_IsCorrect ? "正确" : "错误",
                    type4: q => q.Qus_Answer,
                    type5: q => q.Qus_Items.length === 1 ?
                        q.Qus_Items[0].Ans_Context : q.Qus_Items.map((m, i) => `${i + 1}、${m.Ans_Context}`).join('；'),
                },
                template: `<div class="answer">
                        正确答案：<span v-html="answer(ques)"></span>
                    </div>`
            },          
        }
    });
});