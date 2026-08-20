//试题的难度
Vue.component('ques_diff', {
    props: ['ques'],
    data: function () {
        return {
            diffs: ['很简单', '简单', '有难度', '比较难', '很难'],
            loading: false
        }
    },
    watch: {

    },
    computed: {},
    mounted: function () {

    },
    methods: {

    },
    template: `<div class="ques_diff">  
       {{diffs[ques.Qus_Diff-1]}}
    </div> `
});