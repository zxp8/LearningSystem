$ready(['/Utilities/Components/question/review.js',
    '/Utilities/Components/question/function.js',
    'Components/group.js'], function () {
        window.vapp = new Vue({
            el: '#vapp',
            data: {
                //考试id和成绩id
                examid: $api.querystring('examid', 0),
                exrid: $api.querystring('exrid', 0),

                student: {},     //当前参考的学员，有可能不是当前学员
                platinfo: {},
                organ: {},
                config: {},      //当前机构配置项    

                exam: {},
                result: {},         //考试成绩的籹据实体对象
                exrxml: {},          //答题信息，xml
                paper: {},           //试卷信息              
                types: {},          //题型

                scoreFinal: 0,       //考试得分

                tabactive: '',      //选项卡的状态
                error: '',           //错误提示信息，例如不能查看考虑成绩时

                init: false,        //是否初始化完成
                loading: false
            },
            mounted: function () {
                //window.addEventListener('scroll', this.handleScroll, true);
                var th = this;
                th.organ = window.org;
                th.config = window.config;

                th.loading = true;
                $api.bat(
                    $api.cache('Question/Types:9999'),
                    $api.cache('Platform/PlatInfo:60'),
                    $api.cache('Exam/ForID', { 'id': th.examid }),
                    $api.get('Exam/ResultReview', { 'id': th.exrid })
                ).then(([types, plat, exam, result]) => {
                    //获取结果           
                    th.types = types.data.result;
                    th.platinfo = plat.data.result;
                    th.config = $api.organ(th.organ).config;
                    th.exam = exam.data.result;
                    if (th.exam == null) throw '考试不存在！';
                    if (!th.exam.Exam_IsAllowReview) throw '禁止查看考试成绩详情！';
                    th.result = result.data.result;
                    th.scoreFinal = th.result.Exr_ScoreFinal;
                    //解析答题信息
                    th.exrxml = $api.loadxml(th.result.Exr_Results);
                    //console.log('答题信息：');
                    //console.log(th.exrxml);
                    th.loading = true;
                    let paperapi = th.exam.Exam_Purpose == 0 ? 'TestPaper/ForID' : 'ExamTestPaper/ForID';
                    $api.bat(
                        $api.cache('Account/ForID', { 'id': th.result.Ac_ID }),
                        $api.cache(paperapi, { 'id': th.result.Tp_Id })
                    ).then(([student, paper]) => {
                        //获取结果
                        th.student = student.data.result;
                        th.paper = paper.data.result;
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
                }).catch(function (err) {
                    th.error = err;
                    alert(err);
                    console.error(err);
                }).finally(() => th.loading = false);
            },
            created: function () {
                var th = this;
                window.interval_number = window.setInterval(function () {
                    let ques = $dom('card[qid]');   //试题数
                    let render = $dom('card[render=true]');    //已渲染的试题数
                    if (ques.length > 0 && ques.length == render.length) {
                        th.init = true;
                        window.clearInterval(window.interval_number);
                    }
                }, 200);
            },
            computed: {
                //试卷中的答题信息
                //返回结构：先按试题分类，分类下是答题信息
                questions: function () {
                    var exrxml = this.exrxml;
                    var arr = [];
                    if (JSON.stringify(exrxml) === '{}') return arr;
                    var elements = exrxml.getElementsByTagName("ques");
                    for (var i = 0; i < elements.length; i++) {
                        var gruop = $dom(elements[i]);
                        //题型,题量，总分
                        let type = Number(gruop.attr('type'));
                        let count = Number(gruop.attr('count'));
                        let number = Number(gruop.attr('number'));
                        let byname = gruop.attr('byname');
                        //试题
                        var qarr = [];
                        var list = gruop.find('q');
                        for (var j = 0; j < list.length; j++) {
                            let q = $dom(list[j]);
                            let qid = q.attr('id');
                            let ans = q.attr('ans');
                            //如果是简答题，答题内容与节点文本
                            if (type == 4 || type == 5) ans = q.text();
                            let num = Number(q.attr('num'));
                            let sucess = q.attr('sucess') == 'true';
                            let score = Number(q.attr('score'));
                            qarr.push({
                                'id': qid, 'type': type, 'num': num,
                                'ans': ans, 'success': sucess, 'score': score
                            });
                        }
                        arr.push({
                            'type': type, 'byname': byname, 'count': count, 'number': number, 'ques': qarr
                        });
                    }
                    return arr;
                },
                //总题数
                ques_all_count: function () {
                    var count = 0;
                    for (var i = 0; i < this.questions.length; i++)
                        count += this.questions[i].ques.length;
                    return count;
                },
                //试卷满分
                totalScore: function () {
                    if (this.paper == null) return 0;
                    return this.exam.Exam_Purpose == 0 ? this.paper.Tp_Total : this.paper.Etp_Total;
                },
                //及格分
                passScore: function () {
                    if (this.paper == null) return 0;
                    return this.exam.Exam_Purpose == 0 ? this.paper.Tp_PassScore : this.paper.Etp_PassScore;
                },
                //答对的题数
                ques_success_count: function () {
                    var count = 0;
                    for (var i = 0; i < this.questions.length; i++) {
                        var ques = this.questions[i].ques;
                        for (var j = 0; j < ques.length; j++) {
                            if (ques[j].success) count++;
                        }
                    }
                    return count;
                },
                //答错的题数
                ques_error_count: function () {
                    var count = 0;
                    for (var i = 0; i < this.questions.length; i++) {
                        var ques = this.questions[i].ques;
                        for (var j = 0; j < ques.length; j++) {
                            if (!ques[j].success) count++;
                        }
                    }
                    return count;
                },
                //未做的题数
                ques_unanswerd_count: function () {
                    var count = 0;
                    for (var i = 0; i < this.questions.length; i++) {
                        var ques = this.questions[i].ques;
                        for (var j = 0; j < ques.length; j++) {
                            if (ques[j].ans == '') count++;
                        }
                    }
                    return count;
                },
            },
            watch: {
                'tabactive': function (nv, ov) {
                    //console.log(nv);
                }
            },
            methods: {
                //得分样式
                scoreStyle: function (score) {
                    //总分和及格分
                    var total = this.exam ? this.exam.Exam_Total : -1;
                    var passscore = this.paper ? this.paper.Tp_PassScore : -1;
                    if (score == total) return "praise";
                    if (score < passscore) return "nopass";
                    if (score < total * 0.8) return "general";
                    if (score >= total * 0.8) return "fine";
                    return "";
                }
            }
        });
    });
