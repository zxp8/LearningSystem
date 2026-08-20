
$ready(
    ['/Utilities/Components/question/function.js',
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

                loading: false
            },
            watch: {},
            updated: function () {
                this.$mathjax();
            },
            created: function () { },
            mounted: function () { },
            methods: {
                //验证方法
                verify: function (ques, alert) {
                    var th = this;
                    //答案不得为空
                    if (!ques.Qus_Answer || ques.Qus_Answer == '') {
                        return alert('简答题的答案不得为空！', 0);
                    }
                    return true;
                },
                //试题加载完成
                quesload: function (ques) {
                    this.entity = ques;
                    //重置题干的编辑框
                    let editor = this.$refs['editor_title'];
                    if (editor != null) editor.setContent(ques.Qus_Title);
                    //重置答案的编辑框
                    let answer = this.$refs['editor_answer'];
                    if (answer != null) answer.setContent(ques.Qus_Answer);
                },
            },
        });
    }
);
