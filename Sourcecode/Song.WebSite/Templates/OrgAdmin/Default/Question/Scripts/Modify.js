
$ready(['Components/ques_type.js'], function () {
    window.vapp = new Vue({
        el: '#vapp',
        data: {
            id: $api.querystring('id'),
            org: {},
            config: {},      //当前机构配置项    
            types: [],        //试题类型，来自web.config中配置项

            entity: {},         //当前试题实体
            loading: false,        
        },
        watch: {
        },
        created: function () {
            var th = this;
            th.org = window.org;
            th.config = window.config;
            $api.cache('Question/Types:99999').then(req => {
                if (req.data.success) {
                    th.types = req.data.result;
                    th.getEntity();
                }
            });
        },
        mounted: function () {

        },
        methods: {
            //获取试题
            getEntity: function () {
                var th = this;
                if (th.id == '') return;
                th.loading = true;
                $api.put('Question/ForID', { 'id': th.id }).then(function (req) {
                    if (req.data.success) {
                        var result = req.data.result;
                        th.entity = result;
                        th.gourl(th.entity.Qus_Type, th.types[th.entity.Qus_Type - 1]);
                    } else {
                        throw '未查询到数据';
                    }
                }).catch(err => alert(err, '错误'))
                    .finally(() => th.loading = false);
            },
            //转向
            //type:试题类型的数值
            //typename:试题类型的名称，如单选题、多选题
            gourl: function (type, typename) {
                var url = "Modify_Type" + type;
                var params = $api.url.params();
                for (let i = 0; i < params.length; i++)
                    url = $api.url.set(url, params[i].key, params[i].val);
                url = $api.url.set(url, 'typename', encodeURIComponent(typename));
                if (this.id != '') url = $api.url.dot(this.id, url);
                var loading = this.$fulloading();
                window.setTimeout(function () {
                    window.location.href = url;
                }, 500);
            }
        },

    });
});
