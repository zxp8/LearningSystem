//试题收藏
//事件change：状态，试题
Vue.component('ques_collect', {
    //ques:试题对象
    //accid:管理员的账户ID    
    props: ['ques', 'accid'],
    data: function () {
        return {
            collected: false,   //是否已收藏
            qusid: 0,        //当前试题id
            loading: false
        }
    },
    watch: {
        //监听试题对象
        'ques': {
            handler(nv, ov) {
                if ($api.isnull(nv)) return;
                if (this.qusid > 0) return;
                this.qusid = nv.Qus_ID;
                this.getstate(nv);
            }, immediate: true,
        }
    },
    computed: {},
    mounted: function () {

    },
    methods: {
        //获取收藏状态   
        getstate: function () {
            var th = this;
            th.loading = true;
            $api.get("ExamQues/Collected", { "accid": th.accid, "qusid": th.ques.Qus_ID })
                .then(req => {
                    if (req.data.success) {
                        th.collected = req.data.result;
                    } else {
                        console.error(req.data.exception);
                        throw req.config.way + ' ' + req.data.message;
                    }
                }).catch(err => console.error(err))
                .finally(() => th.loading = false);
        },
        //设置收藏或取消收藏
        setcollect: function () {
            var th = this;
            th.loading = true;
            th.collected = !th.collected;
            $api.post("ExamQues/CollectUpdate", { "accid": th.accid, "qusid": th.ques.Qus_ID, "state": th.collected })
                .then(req => {
                    if (req.data.success) {
                        let result = req.data.result;
                        if (th.collected) {
                            this.$notify({
                                title: '收藏成功',
                                message: '收藏该试题',
                                type: 'success', duration: 1000
                            });
                        } else {
                            this.$notify({
                                title: '取消收藏',
                                message: '试题取消收藏',
                                type: 'warning', duration: 1000
                            });
                        }
                        th.$emit('change', th.ques, th.collected);
                    } else {
                        console.error(req.data.exception);
                        throw req.config.way + ' ' + req.data.message;
                    }
                }).catch(err => console.error(err))
                .finally(() => th.loading = false);
        },
        //改变显示状态
        changeshow: function (ques, state) {
            if (ques.Qus_ID != this.ques.Qus_ID) return;
            if (this.collecte == state) return;          
            this.collected = state;
        }
    },
    template: `<div class="ques_collect">
        <loading v-if="loading" asterisk></loading>
        <el-link v-else-if="!collected" type="warning" plain @click="setcollect"> <icon>&#xe747</icon> 收藏</el-link>
        <el-link v-else type="danger" plain @click="setcollect"> <icon>&#xe679</icon>  取消收藏</el-link>
    </div> `
});