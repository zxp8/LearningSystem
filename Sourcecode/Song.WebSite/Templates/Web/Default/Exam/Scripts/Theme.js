$ready(['/Utilities/Scripts/qrcode.js'], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            account: {},     //当前登录账号
            org: {},
            config: {},      //当前机构配置项 
            examid: $api.querystring("id"),      //考试ID

            theme: {},       //考试主题
            exams: [],
            accounttotal: 0,     //需要参考学员总数
            loadstate: {
                get: false,         //加载数据
                exams: false,      //加载考试场次

            }
        },
        mounted: function () {
            this.gettheme();
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
            //主题变化时，这里用于初次加载
            theme: {
                handler: function (newval, oldval) {
                    if (newval == null) return;
                    this.getaccounttotal();
                    //this.onload();
                    //生成二维码
                    this.$nextTick(function () {
                        this.qrcode();
                    });
                }, immediate: true
            }
        },
        methods: {
            //获取考试主题
            gettheme: function () {
                var th = this;
                th.loadstate.get = true;
                $api.get("Exam/ForID", { "id": th.examid })
                    .then(req => {
                        if (req.data.success) {
                            th.theme = req.data.result;
                            th.getaccounttotal();
                            th.getexmas();
                        } else {
                            console.error(req.data.exception);
                            throw req.config.way + ' ' + req.data.message;
                        }
                    }).catch(err => console.error(err))
                    .finally(() => th.loadstate.get = false);
            },
            //获取参考学员的总数
            getaccounttotal: function () {
                var th = this;
                $api.cache('Exam/ScopeTotal', { 'uid': th.theme.Exam_UID })
                    .then(function (req) {
                        if (req.data.success) {
                            th.accounttotal = req.data.result;
                        } else throw req.data.message;
                    }).catch(err => console.error(err));
            },
            //获取“考试场次”
            getexmas: function () {
                var th = this;
                th.loadstate.exams = true;
                $api.get('Exam/Exams', { 'uid': th.theme.Exam_UID }).then(function (req) {
                    if (req.data.success) {
                        th.exams = req.data.result;
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loadstate.exams = false);
            },
            //生成二维码
            qrcode: function () {
                var th = this;
                var box = $dom("#exam-qrcode" + th.theme.Exam_ID);
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
                let path = window.location.origin + "/web/exam/theme";                
                let url= $api.url.set(path, { "id": this.theme.Exam_ID });                
                return url;
            }
        },
        filters: {

        },
        components: {

        }
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
            },
            goexaming: function (exam) {
                var url = $api.url.set("/web/exam/doing", { "id": exam.Exam_ID });
                //window.location.href = "/web/exam/doing?id=" + exam.Exam_ID;
                return url
            }
        },
        template: `<div>
    <div class="item exam_tit"><b>第（{{index+1}}）场.《{{exam.Exam_Name}}》   </b>    
    <a type="button" :examid="exam.Exam_ID" :href="goexaming(exam)">
    参加考试<icon>&#xe6c6</icon>
</a>
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
});