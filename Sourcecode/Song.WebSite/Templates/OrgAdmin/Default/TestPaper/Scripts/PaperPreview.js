$ready([
    '/Utilities/Components/question/preview.js',
    '/Utilities/Components/question/function.js',
    '../Question/Components/ques_type.js',
    'Components/group.js'
], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            tpid: $api.querystring('tpid'), //试卷ID
            paper: {}, //试卷
            types: [],              //试题类型 
            paperQues: [],           //试卷内容（即试题信息）

            course: {}, //课程
            loadstate: {
                init: false,        //初始化
                build: false,         //生成试卷               
            }
        },
        mounted: function () {
            this.getentity();
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
             //试题总数
             questotal: function () {
                var total = 0;
                for (var i = 0; i < this.paperQues.length; i++)
                    total += this.paperQues[i].count;
                return total;
            },
        },
        watch: {

        },
        methods: {
            //获取试卷
            getentity: function () {
                var th = this;
                th.loadstate.init = true;
                var th = this;
                $api.bat(
                    $api.cache('Question/Types:9999'),
                    $api.get('TestPaper/ForID', { 'id': th.tpid })
                ).then(([type, paper]) => {
                    //试题类型
                    th.types = type.data.result;
                    //试卷
                    th.paper = paper.data.result;
                    th.getcourse(th.paper.Cou_ID);
                    th.generatePaper();
                }).catch(err => console.error(err))
                    .finally(() => th.loadstate.init = false);
            },
            //获取课程
            getcourse: function (couid) {
                var th = this;
                $api.get('Course/ForID', { 'id': couid }).then(function (req) {
                    if (req.data.success) {
                        th.course = req.data.result;
                    } else {
                        console.error(req.data.exception);
                        throw req.config.way + ' ' + req.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                });
            },
            //生成试卷内容
            generatePaper: function () {
                if (JSON.stringify(this.paper) == '{}' && this.paper == null) return;
                //if (this.paperQues.length > 0) return;
                var th = this;
                th.loadstate.build = true;
                $api.get('TestPaper/GenerateRandom', { 'tpid': th.tpid }).then(function (req) {
                    if (req.data.success) {
                        let result = req.data.result;
                        for (let i = 0; i < result.length; i++) {                           
                            for (var j = 0; j < result[i].ques.length; j++) {
                                result[i].ques[j]= window.ques.parseAnswer(result[i].ques[j]);
                            }
                        }
                        th.paperQues =result;
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(err => alert(err))
                    .finally(() => th.loadstate.build = false);
            },
            //刷新
            btnfresh:function() {
               this.getentity();
            },
        },
        filters: {

        },
        components: {

        }
    });
});