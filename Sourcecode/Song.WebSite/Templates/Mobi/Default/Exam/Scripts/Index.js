
$ready(['Components/exam_tabs.js'], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            account: {},     //当前登录账号
            platinfo: {},
            org: {},
            config: {},      //当前机构配置项        
            datas: {},
            tabmenu: '',     //选项卡

            loading: true,
            loading_login: false,       //是否请求过登录      
            finished: false,
            total: 0,
            size: 3,
            index: 0,
            //用于查询的字符
            search: {
                'my_exam': '',
                'all_exam': '',
                'score_exam': ''
            },

            myexam: [],      //当前学员今天以及之后的考试
            allexam: [],             //所有考试（此为考试主题)
            scoreexam: []           //成绩回顾
        },
        mounted: function () {

        },
        created: function () {

        },
        computed: {
            //是否登录
            islogin: t => !$api.isnull(t.account)
        },
        watch: {
            'account': {
                handler: function (nv, ov) {
                    //console.error(nv);
                    //this.loading_init = false;
                    //if ($api.isnull(nv)) return;
                    //this.my_exam();
                }, immediate: false
            },
        },
        methods: {
            //当前学员今天以及之后的考试
            my_exam: function () {
                var th = this;
                if ($api.isnull(th.account)) {
                    window.setTimeout(th.my_exam, 200);
                    return;
                }
                th.loading = true;
                th.index++;
                $api.get('Exam/SelfExam4Todaylate', {
                    'acid': th.account.Ac_ID, 'search': th.search.my_exam,
                    'size': th.size, 'index': th.index
                }).then(function (req) {
                    if (req.data.success) {
                        th.total = req.data.total;
                        var result = req.data.result;
                        for (var i = 0; i < result.length; i++) {
                            th.myexam.push(result[i]);
                        }
                        // 数据全部加载完成
                        if (th.myexam.length >= th.total || result.length == 0) {
                            th.finished = true;
                        }
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //获取所有考试
            all_exam: function () {
                var th = this;
                if ($api.isnull(th.account)) {
                    window.setTimeout(th.all_exam, 200);
                    return;
                }
                var form = {
                    'orgid': th.org.Org_ID, 'start': '', 'end': '', 'search': th.search.all_exam, 'use': true,
                    'size': th.size, 'index': ++th.index
                }
                th.loading = true;
                $api.get('Exam/ThemePager', form).then(function (req) {
                    if (req.data.success) {
                        var result = req.data.result;
                        th.total = req.data.total;
                        var result = req.data.result;
                        for (var i = 0; i < result.length; i++) {
                            th.allexam.push(result[i]);
                        }
                        // 数据全部加载完成
                        if (th.allexam.length >= th.total || result.length == 0) {
                            th.finished = true;
                        }
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //成绩回顾的加载
            score_exam: function () {
                var th = this;
                if ($api.isnull(th.account)) {
                    window.setTimeout(th.score_exam, 200);
                    return;
                }
                var form = {
                    'acid': th.account.Ac_ID,
                    'orgid': 0, 'sbjid': -1, 'search': th.search.score_exam,
                    'size': th.size, 'index': ++th.index
                }
                th.loading = true;
                $api.get('Exam/Result4Student', form).then(function (req) {
                    th.loading = false;
                    if (req.data.success) {
                        var result = req.data.result;
                        th.total = req.data.total;
                        var result = req.data.result;
                        for (var i = 0; i < result.length; i++) {
                            th.scoreexam.push(result[i]);
                        }
                        // 数据全部加载完成
                        if (th.scoreexam.length >= th.total || result.length == 0) {
                            th.finished = true;
                        }
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //查询
            onsearch: function (type) {
                this.index = 0;
                this.loading = true;
                this.finished = false;
                this.total = 0;
                this.myexam = [];
                this.allexam = [];
                this.scoreexam = [];

                eval('this.' + type + '')();
            },
            search: function (type) {

            }
        }
    });
    //考试详情（用于“我的考试”）
    Vue.component('exam_data', {
        props: ['exam', 'index', 'account'],
        data: function () {
            return {
                paper: {},       //试卷
                subject: {},     //专业
                loading: false
            }
        },
        watch: {},
        computed: {},
        mounted: function () {
            var th = this;
            if (th.exam.Exam_Purpose != 0) return;
            //获取“试卷详情”
            $api.bat(
                $api.cache('TestPaper/ForID', { 'id': this.exam.Tp_Id }),
                $api.cache('Subject/ForID', { 'id': this.exam.Sbj_ID })
            ).then(([paper, subject]) => {
                //获取结果
                th.paper = paper.data.result;
                th.subject = subject.data.result;              
            }).catch(err => console.error(err));
        },
        methods: {
            goexaming: function (exam) {
                let url = $dom.routepath() + 'doing';
                window.navigateTo($api.url.set(url, { 'id': exam.Exam_ID }));
            }
        },
        template: `<card>
        <card-title>{{index+1}}.《{{exam.Exam_Name ? exam.Exam_Name : exam.Exam_Title}}》
        <button type="button" :examid="exam.Exam_ID" @click="goexaming(exam)">参加考试</button>
        </card-title>
        <card-content>
        <div class="item"> {{exam.Exam_Title}}</div>
          <div class="item">时间：
            <span v-if="exam.Exam_DateType==1">
              <!--准时开始-->
              {{exam.Exam_Date|date("yyyy-M-dd HH:mm")}} （准时开始）
            </span>
            <span v-else>
              <!--区间时间-->
              {{exam.Exam_Date|date("yyyy-M-dd HH:mm")}} 至
              {{exam.Exam_DateOver|date("yyyy-M-dd HH:mm")}} 之间
            </span>
          </div>
          <div class="item" v-if="paper">限时：{{exam.Exam_Span}}分钟 &nbsp; 题量：{{exam.Exam_QuesCount}}道</div>
          <div class="item">总分：{{exam.Exam_Total}}分（{{paper.Exam_PassScore}}分及格）</div>   
          <template v-if="exam?.Exam_Purpose==0">     
            <div class="item" v-if="subject">专业：{{subject.Sbj_Name}} </div>
            <div class="item" v-if="paper">课程：{{paper.Cou_Name}}</div>    
          </template>    
        </card-content>
      </card>`
    });
    //考试主题（用于所有考试）
    Vue.component('exam_theme', {
        props: ['theme', 'index', 'account'],
        data: function () {
            return {
                group: '',
                exams: [],     //考试场次
                loading: false
            }
        },
        watch: {},
        computed: {},
        mounted: function () {
            this.getgroup();
            this.onload();
        },
        methods: {
            //参考人员
            getgroup: function () {
                var th = this;
                if (this.theme.Exam_GroupType == 1) {
                    th.group = '全体学员';
                    return;
                }
                $api.cache('Exam/ScopeInfo', { 'type': this.theme.Exam_GroupType, 'uid': this.theme.Exam_UID })
                    .then(function (req) {
                        if (req.data.success) {
                            th.group = req.data.result;
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(function (err) {
                        alert(err);
                        console.error(err);
                    });
            },
            //获取“考试场次”
            onload: function () {
                var th = this;
                $api.get('Exam/Exams', { 'uid': this.theme.Exam_UID }).then(function (req) {
                    if (req.data.success) {
                        th.exams = req.data.result;
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(function (err) {
                    alert(err);
                    console.error(err);
                });
            }
        },
        template: `<card class="theme">
        <card-title>{{index+1}}.《{{theme.Exam_Title}}》 </card-title>
        <card-content>
        <div class="item">参考人员：{{group}} </div>     
            <theme_item v-for="(e,index) in exams" :exam="e" :index="index" :account="account"></exam_data>
        </card-content>
      </card>`
    });
    //考试主题中的详情（用于所有考试）
    Vue.component('theme_item', {
        props: ['exam', 'index', 'account'],
        data: function () {
            return {
                paper: {},       //试卷
                subject: {},     //专业
                loading: false
            }
        },
        watch: {
        },
        computed: {
        },
        mounted: function () {
            this.onload();
        },
        methods: {
            //获取“试卷详情”
            onload: function () {
                var th = this;
                if (th.exam.Exam_Purpose != 0) return;
                $api.bat(
                    $api.cache('TestPaper/ForID', { 'id': this.exam.Tp_Id }),
                    $api.cache('Subject/ForID', { 'id': this.exam.Sbj_ID })
                ).then(([paper, subject]) => {
                    //获取结果
                    th.paper = paper.data.result;
                    th.subject = subject.data.result;
                }).catch(function (err) {
                    console.error(err);
                });
            }
        },
        template: `<div>
        <div class="item"><b>第（{{index+1}}）场.《{{exam.Exam_Name}}》   </b>    
        </div>           
          <div class="item">时间：
            <span v-if="exam.Exam_DateType==1">
              <!--准时开始-->
              {{exam.Exam_Date|date("yyyy-M-dd HH:mm")}} （准时开始）
            </span>
            <span v-else>
              <!--区间时间-->
              {{exam.Exam_Date|date("yyyy-M-dd HH:mm")}} 至
              {{exam.Exam_DateOver|date("yyyy-M-dd HH:mm")}} 之间
            </span>
          </div>
          <div class="item" v-if="paper">限时：{{exam.Exam_Span}}分钟 &nbsp; 题量：{{exam.Exam_QuesCount}}道</div>
          <div class="item" v-if="paper">总分：{{exam.Exam_Total}}分（{{exam.Exam_PassScore}}分及格）</div>  
          <template v-if="exam.Exam_Purpose==0">     
            <div class="item">专业：{{subject.Sbj_Name}} </div>
            <div class="item" v-if="paper">课程：{{paper.Cou_Name}}</div>    
        </template> 
      </div>`
    });
    //成绩查看的考试项（用于成绩回顾）
    Vue.component('score_item', {
        props: ['result', 'index', 'account'],
        data: function () {
            return {
                paper: {},       //试卷
                subject: {},     //专业
                exam: null,        //考试
                loading: false
            }
        },
        watch: {
            'exam': function (val) {
                //console.error(this.exam);
            }
        },
        computed: {
            tpcount: function () {
                if ($api.isnull(this.exam)) return 0;
                return this.exam.Exam_Purpose == 0 ? this.paper.Tp_Count : this.paper.Etp_Count;
            },
            tpname: function () {
                if ($api.isnull(this.exam)) return '';
                return this.exam.Exam_Purpose == 0 ? this.paper.Tp_Name : this.paper.Etp_Name;
            },
            //考试不存在
            noexam: t => $api.isnull(t.exam)
        },
        mounted: function () {
            var th = this;
            //console.error(this.result);
            $api.cache('Exam/ForID', { 'id': th.result.Exam_ID }).then(req => {
                if (req.data.success) {
                    th.exam = req.data.result;
                    if ($api.isnull(th.exam)) throw '考试不存在！';
                    if (th.exam.Exam_Purpose == 0) {
                        $api.cache('TestPaper/ForID', { 'id': th.exam.Tp_Id }).then(req => th.paper = req.data.result);
                        $api.cache('Subject/ForID', { 'id': th.result.Sbj_ID }).then(req => th.subject = req.data.result);
                    } else
                        $api.cache('ExamTestPaper/ForID', { 'id': th.exam.Etp_Id }).then(req => th.paper = req.data.result);
                } else throw req.data.message;
            }).catch(err => console.error(err))
                .finally(() => { });
        },
        methods: {
            gourl: function () {
                if ($api.isnull(this.exam)) {
                    alert('考试不存在，可能已经删除！');
                    return;
                }
                if (!this.exam.Exam_IsAllowReview) {
                    alert('此考试不允许查看！');
                    return;
                }
                let url = $dom.routepath() + 'Review';
                url = $api.url.set(url, {
                    "examid": this.result.Exam_ID,
                    "exrid": this.result.Exr_ID
                });
                window.navigateTo(url);
            }
        },
        template: `<card @click="gourl">
        <card-title>{{index+1}}.《 {{result.Exam_Name ? result.Exam_Name : result.Exam_Title}}》
        <score :result="result" :exam="exam" :paper="paper"></score>
        </card-title>
        <card-content>
          <div class="item" v-if="!noexam">考试：{{exam.Exam_Title}}</div>      
           <div class="item" v-if="tpname!=''">试卷： {{tpname}}</div>  
            <template v-if="exam?.Exam_Purpose==0">  
                <div class="item">专业：{{subject.Sbj_Name}}</div>  
                <div class="item">课程：{{paper.Cou_Name}}</div>  
            </template>        
          <div class="item" v-if="exam">限时：{{exam.Exam_Span}}分钟 &nbsp; 题量：{{exam.Exam_QuesCount}}道</div>
          <div class="item">交卷时间：{{result.Exr_SubmitTime|date("yyyy-MM-dd HH:mm:ss")}}</div>
          <div class="item">
                <template v-if="!noexam">
                    （满分{{exam.Exam_Total}}分，{{exam.Exam_PassScore}}分及格）
                </template>
                <span title="考试主题" v-if="noexam">
                        <alert>考试不存在，可能被删除，仅留下考试成绩</alert>
                </span>
           </div> 
        </card-content>
      </card>`
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
