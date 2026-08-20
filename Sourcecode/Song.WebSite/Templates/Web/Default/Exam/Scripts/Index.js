$dom.load.css([$dom.path() + 'styles/pagebox.css']);
$ready(["../Components/courses.js",
    "../Components/course.js",
    '../scripts/pagebox.js',
    'Components/exam_tabs.js',
    '/Utilities/Scripts/qrcode.js'],
    function () {
        window.vapp = new Vue({
            el: '#vapp',
            data: {
                account: {},     //当前登录账号
                platinfo: {},
                org: {},
                config: {},      //当前机构配置项        

                tabmenu: 'my_exam',     //选项卡

                loading: false,
                loading_login: false,       //是否请求过登录
                loading_init: true,      //初始数所据加载
                finished: false,

                form: { 'search': '', 'size': 5, 'index': 0 },
                total: 0,

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
                'loading_login': {
                    handler: function (nv, ov) {
                        //console.log("loading_login:" + nv);
                    }, immediate: true
                },

            },
            methods: {
                //当前学员今天以及之后的考试
                my_exam: function (index) {
                    var th = this;
                    if (index != null) this.form.index = index;
                    this.loading = true;
                    var form = $api.clone(this.form);
                    form['acid'] = th.account.Ac_ID;
                    console.log(form);
                    $api.get('Exam/SelfExam4Todaylate', form).then(function (req) {
                        if (req.data.success) {
                            th.total = req.data.total;
                            var result = req.data.result;
                            th.myexam = result;
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
                },
                //获取所有考试
                all_exam: function (index) {
                    if (index != null) this.form.index = index;
                    var th = this;
                    var form = $api.clone(this.form);
                    form['orgid'] = this.org.Org_ID;
                    form['start'] = '';
                    form['end'] = '';
                    form['use'] = true;
                    th.loading = true;
                    $api.get('Exam/ThemePager', form).then(function (req) {
                        if (req.data.success) {
                            var result = req.data.result;
                            th.total = req.data.total;
                            th.allexam = req.data.result;
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
                },
                //成绩回顾的加载
                score_exam: function (index) {
                    if (index != null) this.form.index = index;
                    var th = this;
                    var form = $api.clone(this.form);
                    form['acid'] = this.account.Ac_ID;
                    form['orgid'] = -1;
                    form['sbjid'] = -1;
                    th.loading = true;
                    $api.get('Exam/Result4Student', form).then(function (req) {
                        if (req.data.success) {
                            th.total = req.data.total;
                            th.scoreexam = req.data.result;
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
                },
                //查询
                onsearch: function (search, tabitem) {
                    this.loading = true;
                    this.total = 0;
                    this.myexam = [];
                    this.allexam = [];
                    this.scoreexam = [];

                    this.form.search = search;
                    eval('this.' + tabitem.tag + '')(1);
                }
            }
        });
        //考试详情（用于“我的考试”）
        Vue.component('my_exam', {
            //exam:考试场次
            props: ['exam', 'index', 'account'],
            data: function () {
                return {
                    paper: {},       //试卷
                    subject: {},     //专业
                    loading: false
                }
            },
            watch: {//主题变化时，这里用于初次加载
                exam: {
                    handler: function (newval, oldval) {
                        if (newval == null) return;
                        this.getpaper();
                    }, immediate: true
                }
            },
            computed: {
                //题量
                tpcount: function () {
                    return this.exam.Exam_Purpose == 0 ? this.paper.Tp_Count : this.paper.Etp_Count;
                }
            },
            mounted: function () { },
            methods: {
                //获取试卷
                getpaper: function () {
                    var th = this;
                    th.loading = true;
                    let apiget = th.exam.Exam_Purpose == 0 ?
                        $api.get('TestPaper/ForID', { 'id': th.exam.Tp_Id }) :
                        $api.get('ExamTestPaper/ForID', { 'id': th.exam.Etp_Id });
                    apiget.then(function (req) {
                        if (req.data.success) {
                            th.paper = req.data.result;
                        } else {
                            console.error(req.data.exception);
                            console.error(th.exam);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
                },
                goexaming: function (exam) {
                    var url = $api.url.set("/web/exam/doing", { "id": exam.Exam_ID });
                    //window.location.href = "/web/exam/doing?id=" + exam.Exam_ID;
                    return url
                }
            },
            template: `<card  shadow="hover" class="my_exam">
            <card-title>
                <span>{{index+1}}.《{{exam.Exam_Name}}》</span>
                <a type="button" :examid="exam.Exam_ID" :href="goexaming(exam)">
                    参加考试<icon>&#xe6c6</icon>
                </a>
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
            <div class="item" v-if="paper">限时：{{exam.Exam_Span}}分钟 &nbsp; 题量：{{tpcount}}道</div>
            <div class="item">总分：{{exam.Exam_Total}}分（{{exam.Exam_PassScore}}分及格）</div>
            <template v-if="exam?.Exam_Purpose==0">     
                <div class="item" v-if="paper">专业：{{paper.Sbj_Name}} </div>
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
                    accounttotal: 0,    //参考学员的数
                    exams: [],     //考试场次
                    loading: false
                }
            },
            watch: {
                //主题变化时，这里用于初次加载
                theme: {
                    handler: function (newval, oldval) {
                        if (newval == null) return;
                        this.getaccounttotal();
                        this.onload();
                        //生成二维码
                        this.$nextTick(function () {
                            this.qrcode();
                        });
                    }, immediate: true
                }
            },
            computed: {},
            mounted: function () { },
            methods: {
                //获取参考学员的总数
                getaccounttotal: function () {
                    var th = this;
                    $api.cache('Exam/ScopeTotal', { 'uid': this.theme.Exam_UID })
                        .then(function (req) {
                            if (req.data.success) {
                                th.accounttotal = req.data.result;
                            } else throw req.data.message;
                        }).catch(err => console.error(err));
                },
                //获取“考试场次”
                onload: function () {
                    var th = this;
                    th.loading = true;
                    $api.get('Exam/Exams', { 'uid': this.theme.Exam_UID }).then(function (req) {
                        if (req.data.success) {
                            th.exams = req.data.result;
                        } else {
                            console.error(req.data.exception);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
                },
                //生成二维码
                qrcode: function () {
                    var th = this;
                    var box = $dom("#course-qrcode" + th.theme.Exam_ID);
                    if (box.length < 1) window.setTimeout(this.qrcode, 200);
                    box.each(function () {
                        if ($dom(this).find("img").length > 0) return;
                        new QRCode(this, {
                            text: th.builderurl(),
                            width: 75, height: 75,
                            colorDark: "#000000", colorLight: "#ffffff",
                            render: "canvas", correctLevel: QRCode.CorrectLevel.L
                        });
                    });
                },
                //生成链接
                builderurl: function () {
                    let path = window.location.origin + $dom.route();
                    path = path.substring(0, path.lastIndexOf('/') + 1) + 'theme';
                    return $api.url.set(path, { "id": this.theme.Exam_ID });
                }
            },
            template: `<card class="theme"  shadow="hover">
        <card-title>
            <span>{{index+1}}.<a :href='builderurl()' target="_blank">《{{theme.Exam_Title}}》 </a> </span>
            <el-tag v-if="theme.Exam_GroupType == 1">全体学员</el-tag>
            <el-tag type="success" v-if="theme.Exam_GroupType == 2">限定学员组</el-tag>
            <el-tag type="warning" v-if="theme.Exam_GroupType == 3">指定学员</el-tag>
            <el-tooltip effect="light" content="Bottom Right 提示文字" placement="bottom-end">
                <icon large>&#xa053</icon>
                <div slot="content"><div :id="'course-qrcode'+theme.Exam_ID"></div></div>
            </el-tooltip>           
        </card-title>
        <card-content>        
            <theme_item v-for="(e,index) in exams" :exam="e" :theme="theme" :index="index" :account="account"></theme_item>
        </card-content>
      </card>`
        });
        //考试主题中的详情（用于所有考试）
        Vue.component('theme_item', {
            props: ['exam', 'theme', 'index', 'account'],
            data: function () {
                return {
                    paper: {},       //试卷
                    subject: {},     //专业
                    loading: false
                }
            },
            watch: {
                //主题变化时，这里用于初次加载
                exam: {
                    handler: function (newval, oldval) {
                        if (newval == null) return;
                        this.getpaper();
                    }, immediate: true
                }
            },
            computed: {
                //题量
                tpcount: function () {
                    return this.exam.Exam_Purpose == 0 ? this.paper.Tp_Count : this.paper.Etp_Count;
                }
            },
            methods: {
                //获取试卷
                getpaper: function () {
                    var th = this;
                    th.loading = true;
                    let apiget = th.exam.Exam_Purpose == 0 ?
                        $api.get('TestPaper/ForID', { 'id': th.exam.Tp_Id }) :
                        $api.get('ExamTestPaper/ForID', { 'id': th.exam.Etp_Id });
                    apiget.then(function (req) {
                        if (req.data.success) {
                            th.paper = req.data.result;
                        } else {
                            console.error(req.data.exception);
                            console.error(th.exam);
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
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
          <div class="item" v-if="paper">限时：{{exam.Exam_Span}}分钟 &nbsp; 题量：{{tpcount}}道</div>
          <div class="item" v-if="paper">总分：{{exam.Exam_Total}}分（{{exam.Exam_PassScore}}分及格）</div>
      </div>`
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
            watch: {},
            computed: {
                tpcount: function () {
                    return this.exam.Exam_Purpose == 0 ? this.paper.Tp_Count : this.paper.Etp_Count;
                },
                tpname: function () {
                    return this.exam.Exam_Purpose == 0 ? this.paper.Tp_Name : this.paper.Etp_Name;
                }
            },
            mounted: function () {
                var th = this;
                $api.cache('Exam/ForID', { 'id': th.result.Exam_ID }).then(req => {
                    if (req.data.success) {
                        th.exam = req.data.result;
                        if (th.exam.Exam_Purpose == 0) {
                            $api.cache('TestPaper/ForID', { 'id': th.exam.Tp_Id }).then(req => th.paper = req.data.result);
                            $api.cache('Subject/ForID', { 'id': th.result.Sbj_ID }).then(req => th.subject = req.data.result);
                        } else
                            $api.cache('ExamTestPaper/ForID', { 'id': th.exam.Etp_Id }).then(req => th.paper = req.data.result);
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                });
            },
            methods: {
                //得分样式
                scoreStyle: function (score) {
                    //总分和及格分
                    var total = this.exam.Exam_Total;
                    var passscore = this.paper ? this.paper.Tp_PassScore : -1;
                    if (score == total) return "praise";
                    if (score < passscore) return "nopass";
                    if (score < total * 0.8) return "general";
                    if (score >= total * 0.8) return "fine";
                    return "";
                },
                gourl: function () {
                    var url = $api.url.set("/student/exam/review", {
                        "examid": this.result.Exam_ID,
                        "exrid": this.result.Exr_ID
                    });
                    var obj = {
                        'url': url, 'ico': 'e6ef',
                        'title': this.exam.Exam_Name,
                        'width': '80%',
                        'height': '80%'
                    };
                    obj['showmask'] = true; //始终显示遮罩
                    obj['min'] = false;
                    $pagebox.create(obj).open();
                }
            },
            template: `<card shadow="hover">
        <card-title style="cursor: pointer" @click="gourl">{{index+1}}.《 {{exam.Exam_Title}} 》- {{exam.Exam_Name}}
            <score :class="scoreStyle(result.Exr_ScoreFinal)">{{result.Exr_ScoreFinal}} 分</score>
        </card-title>
        <card-content>
            <div class="item">试卷： {{tpname}}</div>  
            <template v-if="exam.Exam_Purpose==0">  
                <div class="item">专业：{{subject.Sbj_Name}}</div>  
                <div class="item">课程：{{paper.Cou_Name}}</div>  
            </template>          
            <div class="item">限时：{{exam.Exam_Span}}分钟 &nbsp; 题量：{{tpcount}}道</div>
            <div class="item">交卷时间：{{result.Exr_SubmitTime|date("yyyy-MM-dd HH:mm:ss")}}</div>
            <div class="item">得分：{{result.Exr_ScoreFinal}} 分
            （满分{{exam.Exam_Total}}分，{{exam.Exam_PassScore}}分及格）</div>           
        </card-content>
      </card>`
        });
    });
