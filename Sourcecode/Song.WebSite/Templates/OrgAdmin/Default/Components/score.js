//用于显示成绩，支持小数点对齐
Vue.component('score', {
    // 属性
    //value:得分
    //passscore:及格分数
    props: ['value', 'passscore'],
    data: function () {
        return {
            loading: false
        }
    },
    watch: {

    },
    computed: {
        //显示的分数
        score: function () {
            if (!this.value) return 0;
            return Math.round(this.value * 100) / 100;
        },
        //前缀，小数点之前的数字
        prev: function () {
            return Math.floor(this.score);
        },
        after: function () {
            let decimal = this.score % 1;
            if (decimal <= 0) return '';
            return this.score.toString().split('.')[1];
        },
        dot: function () {
            return this.after != '' ? '.' : '';
        },

    },
    mounted: function () {

    },
    methods: {

    },
    template: `<div :class="{'red':value<passscore,'score':true}" @click="$emit('click')">  
                <span class="prev">{{prev}}</span>
                <span class="dot" v-html="dot"></span>
                <span class="after">{{after}}</span>
                <div><slot></slot></div>
    </div>`
});