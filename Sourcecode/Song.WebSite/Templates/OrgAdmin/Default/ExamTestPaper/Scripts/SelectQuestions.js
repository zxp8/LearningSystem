$ready([
    '/Utilities/Components/question/function.js',
    '../Question/Components/ques_type.js',      //题型
    '../ExamQues/Components/ques_diff.js',      //难度
    '../ExamQues/Components/ques_collect.js',   //收藏
    '../ExamQues/Components/selecttag.js',  //标签
    'Components/selectques.js',        //试题行
    'Components/diff.js',      //难度选择
    'Components/selectparts.js',        //试题分类的选择
    'Components/selectknl.js',
    'Components/selecttags.js',
], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            org: window.org,      //当前机构对象     
            type: $api.querystring('type'),    //当前题型
            types: $api.querystring('types').split(','),       //所有题型
            admin: {},          //当前登录用户

            tabs: [
                { name: '按分类选题', tab: 'parts', icon: 'a015' },
                { name: '按关键字选题', tab: 'tags', icon: 'e841' },
                { name: '按知识点选题', tab: 'knls', icon: 'e6fd' },
                { name: '我的收藏', tab: 'collect', icon: 'e747' },
            ],
            activeName: 'parts',
            //选中的试题,试题id
            //selectedarr: $api.querystring('ques', '').split(',').filter(item => item !== ''),
            selectedques: [],       //选中的试题
            //按试题分类查询条件
            partsform: {
                "orgid": -1, "search": "", "isdeleted": false, "qpid": "", "tagid": "", "knlid": "", "type": $api.querystring('type'),
                "diff": "", "use": true, "error": false, 'wrong': false, "size": 10, "index": 1
            },
            partstotal: 1, //总记录数
            partspages: 1, //总页数
            partsques: [],

            //按关键字查询条件
            tagsform: {
                "orgid": -1, "search": "", "isdeleted": false, "qpid": "", "tagid": "", "knlid": "", "type": $api.querystring('type'),
                "diff": "", "use": true, "error": false, 'wrong': false, "size": 10, "index": 1
            },
            tagstotal: 1, //总记录数
            tagspages: 1, //总页数
            tagsques: [],

            //知识点
            knlsform: {
                "orgid": -1, "search": "", "isdeleted": false, "qpid": "", "tagid": "", "knlid": "", "type": $api.querystring('type'),
                "diff": "", "use": true, "error": false, 'wrong': false, "size": 10, "index": 1
            },
            knltotal: 1, //总记录数
            knlpages: 1, //总页数
            knlsques: [],

            //收藏试题的查询条件
            collectform: {
                "acid": "", "search": "", "qpid": "", "tagid": "", "knlid": "", "type": $api.querystring('type'),
                "diff": "", "use": true, "error": false, "wrong": false, "size": 10, "index": 1
            },
            collectques: [],
            collecttotal: 1, //总记录数
            collecttotalpages: 1, //总页数


            loadstate: {
                init: false,        //初始化
                parts: false,         //试题分类的加载
                knls: false,         //知识点预载
                tags: false,      //关键字预载
                collect: false          //我的收藏
            }
        },
        updated: function () {
            this.$mathjax();
        },
        mounted: function () {
            var th = this;
            this.partsform.orgid = window.org.Org_ID;
            this.tagsform.orgid = window.org.Org_ID;
            this.knlsform.orgid = window.org.Org_ID;

            //当前登录的管理员
            $api.login.current('admin', d => {
                th.admin = d;
                th.collectform.acid = d.Acc_Id;
                th.getcollectques(1, true);
            });

            this.receive();
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
            //试题分类的试题
            getpartques: function (index, parts) {
                var th = this;
                th.loadstate.parts = true;
                if (index != null) this.partsform.index = index;
                if (parts != null) this.partsform.qpid = parts.map(user => user.Qp_ID).join(',');
                $api.get("ExamQues/QuesPager", th.partsform).then(function (d) {
                    if (d.data.success) {
                        var result = d.data.result;
                        for (let i = 0; i < result.length; i++) {
                            result[i] = window.ques.parseAnswer(result[i]);
                            //试题是否选中
                            result[i]["checked"] = th.selectedques.some(m => m.Qus_ID == result[i].Qus_ID);
                            result[i]["showdetail"] = true;
                            //result[i].Qus_Title = result[i].Qus_Title.replace(/(<([^>]+)>)/ig, "");
                        }

                        th.partsques = result;
                        th.partspages = Number(d.data.totalpages);
                        th.partstotal = d.data.total;
                    } else {
                        throw d.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                }).finally(() => th.loadstate.parts = false);
            },
            //按知识点获取试题
            getknlques: function (index, knls) {
                var th = this;
                th.loadstate.knls = true;
                if (index != null) this.knlsform.index = index;
                if (knls != null) this.knlsform.knlid = knls.map(t => t.Qk_ID).join(',');
                $api.get("ExamQues/QuesPager", th.knlsform).then(function (d) {
                    if (d.data.success) {
                        var result = d.data.result;
                        for (let i = 0; i < result.length; i++) {
                            result[i] = window.ques.parseAnswer(result[i]);
                            //试题是否选中
                            result[i]["checked"] = th.selectedques.some(m => m.Qus_ID == result[i].Qus_ID);
                            result[i]["showdetail"] = true;
                            //result[i].Qus_Title = result[i].Qus_Title.replace(/(<([^>]+)>)/ig, "");
                        }

                        th.knlsques = result;
                        th.knlpages = Number(d.data.totalpages);
                        th.knltotal = d.data.total;
                    } else {
                        throw d.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                }).finally(() => th.loadstate.knls = false);
            },
            //按关键字获取试题
            gettagsques: function (index, tags) {
                var th = this;
                th.loadstate.tags = true;
                if (index != null) this.tagsform.index = index;
                if (tags != null) this.tagsform.tagid = tags.map(t => t.Qtag_ID).join(',');
                $api.get("ExamQues/QuesPager", th.tagsform).then(function (d) {
                    if (d.data.success) {
                        var result = d.data.result;
                        for (let i = 0; i < result.length; i++) {
                            result[i] = window.ques.parseAnswer(result[i]);
                            //试题是否选中
                            result[i]["checked"] = th.selectedques.some(m => m.Qus_ID == result[i].Qus_ID);
                            result[i]["showdetail"] = true;
                            //result[i].Qus_Title = result[i].Qus_Title.replace(/(<([^>]+)>)/ig, "");
                        }
                        th.tagsques = result;
                        th.tagspages = Number(d.data.totalpages);
                        th.tagstotal = d.data.total;
                    } else {
                        throw d.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                }).finally(() => th.loadstate.tags = false);
            },
            //收藏的试题
            getcollectques: function (index, show) {
                var th = this;
                th.loadstate.collect = true;
                if (index != null) this.collectform.index = index;
                $api.get("ExamQues/CollectPager", th.collectform).then(function (d) {
                    if (d.data.success) {
                        var result = d.data.result;
                        for (let i = 0; i < result.length; i++) {
                            result[i] = window.ques.parseAnswer(result[i]);
                            //试题是否选中
                            result[i]["checked"] = th.selectedques.some(m => m.Qus_ID == result[i].Qus_ID);
                            result[i]["showdetail"] = true;
                            //result[i].Qus_Title = result[i].Qus_Title.replace(/(<([^>]+)>)/ig, "");
                        }
                        th.collectques = result;
                        th.collecttotalpages = Number(d.data.totalpages);
                        th.collecttotal = d.data.total;
                        //如果有收藏的试题，则优先显示收藏的试题
                        if (show == true && th.collectques.length > 0) {
                            th.activeName = 'collect';
                        }
                    } else {
                        throw d.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                }).finally(() => th.loadstate.collect = false);
            },
            //接收的主窗体数据
            receive: function () {
                return new Promise((resolve, reject) => {
                    //像主窗体传值，传三个值：选中的分类，选中的试题数，调用函数名
                    var pagebox = window.top.$pagebox;
                    if (pagebox && pagebox.source.top) {
                        [this.selectedques] = pagebox.source.box(window.name, 'vapp.transmitques', false, this.type);
                        resolve(this.entity);
                    }
                });
            },
            //收藏试题状态变更时
            collect: function (ques, state) {
                //重新加载收藏的试题列表
                this.getcollectques(null, false);

                let components=this.$refs['selectques'];
                if (components && components.length > 0) {
                    for (let i = 0; i < components.length; i++) {
                        components[i].setcollect(ques, state);
                    }
                }
                //console.error(components);
            },
            //试题选中内容变更,并向主窗体传值
            changeselect: function (q, checked) {
                if (checked) this.selectedques.push(q);
                else this.selectedques = this.selectedques.filter(item => item.Qus_ID !== q.Qus_ID);
                //像主窗体传值，当前实体，图片对象
                var pagebox = window.top.$pagebox;
                if (pagebox && pagebox.source.box) {
                    pagebox.source.box(window.name, 'vapp.receiveques', false, [this.selectedques, this.type]);
                    //let curbox = pagebox.source.self(window.name);
                    //curbox.shut();
                }
            }

        },
        filters: {

        },
        components: {

        }
    });
});