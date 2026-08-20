$ready(function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            organ: {},
            config: {},      //当前机构配置项      
            form: {
                orgid: -1,
                start: '', end: '',
                use: null, ismanual: null,
                search: '',
                size: 8, index: 1
            },
            total: 1, //总记录数
            totalpages: 1, //总页数
            datas: [],

            loadstate: {
                init: true,        //初始化
                def: false,         //默认
                get: false,         //加载数据               
            }
        },
        mounted: function () {

        },
        created: function () {
            var th = this;
            th.org = window.org;
            th.config = window.config;
            th.form.orgid = th.organ.Org_ID;
            th.handleCurrentChange();
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
            //选择时间区间
            selectDate: function (start, end) {
                this.form.start = start;
                this.form.end = end;
                this.handleCurrentChange(1);
            },
            //加载数据页
            handleCurrentChange: function (index) {
                if (index != null) this.form.index = index;
                var th = this;
                th.loadstate.get = true;
                //每页多少条，通过界面高度自动计算
                let area = $dom.height() - 108;
                th.form.size = Math.round(area / 41);
                $api.get('Exam/ExamAdminPager', this.form).then(function (req) {
                    if (req.data.success) {
                        th.datas = req.data.result;
                        th.totalpages = Number(req.data.totalpages);
                        th.total = req.data.total;
                    } else {
                        console.error(req.data.exception);
                        throw req.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loadstate.get = false);
            },
            //打开人工批阅
            btnResultManual: function (row) {
                let file = $api.url.set('ResultsManual', { 'id': row.Exam_ID });
                let boxid = file + "_" + row.Exam_ID;
                let title = '考试：《' + row.Exam_Title + "》";
                this.$refs.btngroup.pagebox(file, title, boxid, 1000, '80%', { 'ico': 'a02e' });
            },
        },
        filters: {

        },
        components: {
            //参加考试的人次
            'stnumber': {
                props: ['examid'],
                data: function () {
                    return {
                        number: 0,
                        loading: false
                    }
                },
                watch: {},
                created: function () {
                    var th = this;
                    th.loading = true;
                    $api.get("Exam/AttendCount", { 'examid': th.examid }).then(function (req) {
                        if (req.data.success) {
                            let result = req.data.result;     
                            th.number = result.number;
                        } else {
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                        .finally(() => th.loading = false);
                },
                methods: {},
                template: `<div class="stnumber">
                <loading v-if="loading"></loading>
                <span v-else>{{number}}</span>
                    </div>`
            }
        }
    });
});

