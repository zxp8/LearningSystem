
$ready([], function () {
window.vapp = new Vue({
        el: '#vapp',
        data: {
            orgid: 0,
            form: {
                'orgid': '', 'stsid': '', 'couid': '',
                'acc': '', 'name': '', 'idcard': '', 'mobi': '', 'start': '', 'end': '',
                'size': '10', 'index': '1'
            },
            datas: [],       //数据集
            total: 1, //总记录数
            totalpages: 1, //总页数

            loading: false
        },
        watch: {

        },
        computed: {

        },
        mounted: function () {
            var th = this;
            //机构配置信息         
            th.organ = window.org;
            th.config = window.config;
            th.form.orgid = th.organ.Org_ID;
            th.handleCurrentChange(1);           
        },
        methods: {
            //选择时间区间
            selectDate: function (start, end) {
                this.form.start = start;
                this.form.end = end;
                //this.handleCurrentChange(1);
            },
            //显示电话
            showTel: function (row) {
                if (row.Ac_MobiTel1 == '' && row.Ac_MobiTel2 == '') return '';
                if (row.Ac_MobiTel1 == '') row.Ac_MobiTel1 = row.Ac_MobiTel2;
                if (row.Ac_MobiTel1 == row.Ac_MobiTel2) return row.Ac_MobiTel1;
                return row.Ac_MobiTel1 + (row.Ac_MobiTel2 != '' ? '/' + row.Ac_MobiTel2 : '');
            },
            //加载数据页
            handleCurrentChange: function (index) {
                if (index != null) this.form.index = index;
                var th = this;
                //每页多少条，通过界面高度自动计算
                let area = $dom.height() - 115;
                th.form.size = Math.floor(area / 41);
                th.loading = true;
                $api.get("Account/purchasepager", th.form).then(function (d) {
                    th.loading = false;
                    if (d.data.success) {
                        th.datas = d.data.result;
                        th.totalpages = Number(d.data.totalpages);
                        th.total = d.data.total;
                    } else {
                        throw d.data.message;
                    }
                }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            },
            //打开窗体
            btnopenbox: function (data) {
                let url = 'Courses?id=' + data.Ac_ID;
                let title = '' + data.Ac_Name + '(' + data.Ac_AccName + ')';
                let boxid = null;
                this.$refs.btngroup.pagebox(url, title, boxid, 900, 600, {
                    'ico': 'e732', 'iconstyle': {
                        'size': -10
                    }
                });
            },
        }
    });
});
