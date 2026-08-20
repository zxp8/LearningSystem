$ready(function () {

    window.vapp = new Vue({
        el: '#vapp',
        data: {
            account: {},     //当前登录账号          
            org: {},
            config: {},      //当前机构配置项        
            //查询项
            form: { 'acid': '', 'orgid': '', 'sbjid': '', 'search': '', 'size': 10, 'index': 0 },
            results: [],        //成绩记录
            total: 1, //总记录数
            totalpages: 1, //总页数

            loading_init: true,
            loading: false
        },
        mounted: function () {
            var th = this;
            th.org = window.org;
            th.config = window.config;
            th.form.orgid = th.org.Org_ID;
            $api.login.current('account', acc => {
                th.account = acc;
                th.form.acid = th.account.Ac_ID;
                th.$refs['query_panel'].setmodel(th.form);
                th.handleCurrentChange(1);
            }, err => { });

        },
        created: function () {

        },
        computed: {
            //是否登录
            islogin: t => JSON.stringify(t.account) != '{}' && t.account != null,

        },
        watch: {
        },
        methods: {
            //加载数据页
            handleCurrentChange: function (index) {
                if (index != null) this.form.index = index;
                var th = this;
                //每页多少条，通过界面高度自动计算
                let area = $dom.height() - 84;
                th.form.size = Math.floor(area / 210);
                th.loading = true;
                var loading = this.$fulloading();
                $api.get("Exam/Result4Student", th.form).then(function (d) {
                    if (d.data.success) {
                        th.results = d.data.result;
                        th.totalpages = Number(d.data.totalpages);
                        th.total = d.data.total;
                        /* //修改得分，方便查看界面效果
                        for (var i = 0; i < th.results.length; i++) {
                            th.results[i].Exr_ScoreFinal = 100;
                        }*/
                        th.$nextTick(function () {
                            loading.close();
                        });
                    } else {
                        console.error(d.data.exception);
                        throw d.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //查看成绩
            review: function (result, exam) {
                if ($api.isnull(exam)) {
                    alert('考试不存在，可能已经删除！');
                    return;
                }
                if (exam != null && !exam?.Exam_IsAllowReview) {
                    alert('此考试不允许查看！');
                    return;
                }
                if (!window.top || !window.top.vapp) return;
                var url = $api.url.set("/student/exam/review", {
                    "examid": result.Exam_ID,
                    "exrid": result.Exr_ID
                });
                var obj = {
                    'url': url,
                    'pid': window.name,
                    'ico': 'e6ef', 'min': false,
                    'title': '成绩查看 - ' + result.Exam_Title,
                    'width': 900,
                    'height': '80%'
                }
                window.top.vapp.open(obj);
            }
        }
    });
    //成绩查看的考试项（用于成绩回顾）
    Vue.component('score_item', {
        props: ['result', 'index', 'account'],
        data: function () {
            return {
                paper: {},       //试卷
                subject: {},     //专业
                exam: {},        //考试
                loading: false
            }
        },
        watch: {
            'result': {
                handler: function (nv, ov) {
                    var th = this;
                    $api.get('Exam/ForID', { 'id': th.result.Exam_ID }).then(req => {
                        th.exam = req.data.result;
                        if (th.exam == null) throw '考试不存在！';
                        if (th.exam.Exam_Purpose == 0) th.getCourseTestpaper();
                        else th.getExamTestpaper();
                    });

                }, immediate: true
            }
        },
        computed: {},
        mounted: function () {

        },
        methods: {
            //获取课程试卷的详情
            getCourseTestpaper: function () {
                var th = this;
                th.loading = true;
                //获取“试卷详情”
                $api.bat(
                    $api.cache('TestPaper/ForID', { 'id': this.result.Tp_Id }),
                    $api.cache('Subject/ForID', { 'id': this.result.Sbj_ID })
                ).then(([paper, subject]) => {
                    th.paper = paper.data.result;
                    th.subject = subject.data.result;
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //获取考试试卷
            getExamTestpaper: function () {
                var th = this;
                th.loading = true;
                //获取“试卷详情”
                $api.bat(
                    $api.cache('ExamTestPaper/ForID', { 'id': this.result.Tp_Id }),
                ).then(([paper]) => {
                    th.paper = paper.data.result;
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
        },
        template: `<slot :paper="paper" :exam="exam" :subject="subject" :th="this"></slot>`
    });
    //成绩得分的显示
    Vue.component("score", {
        //成绩，考试场次对象,试卷对象，专业
        props: ["result", "exam", "paper", "subject"],
        methods: {
            //得分样式
            scoreStyle: function (score) {
                if (this.exam == null) return "";
                if (!this.exam.Exam_IsShowScore) return "noshowscore";
                //总分和及格分
                var total = this.exam ? this.exam.Exam_Total : -1;
                var passscore = this.paper ? this.paper.Tp_PassScore : -1;
                if (score == total) return "praise";
                if (score < passscore) return "nopass";
                if (score < total * 0.8) return "general";
                if (score >= total * 0.8) return "fine";
                return "";
            },
            btnclick: function (result) {
                this.$emit('click', result, this.exam);
            }
        },
        template: `<span score :class="scoreStyle(result.Exr_ScoreFinal)"
                           @click="btnclick(result)">
                        <template v-if="exam==null || exam?.Exam_IsShowScore">
                            {{result.Exr_ScoreFinal}}  分
                        </template>
                        <alert v-else>成绩暂不显示</alert>                           
            </span>`
    })
});
