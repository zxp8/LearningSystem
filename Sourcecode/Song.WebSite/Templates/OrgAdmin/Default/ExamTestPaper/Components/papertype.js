//试卷类型
Vue.component('papertype', {
    //type: 1固定试题 2随机出题
    //showname: 是否显示名称，如果为false，则只显示图标,
    props: ['type', 'showname'],
    data: function () {
        return {
            loading: false
        }
    },
    watch: {

    },
    computed: {

    },
    mounted: function () {

    },
    methods: {

    },
    template: `<span class="papertype" :type="type">
        <el-tooltip v-if="type==2" content="试卷类型：随机出题" placement="bottom">
            <span><icon medium>&#xe6cc</icon> <template v-if="showname==null || showname==true">随机出题</template></span>
        </el-tooltip>
        <el-tooltip v-if="type==1" content="试卷类型：固定试题" placement="bottom">
            <span><icon large>&#xe6cb</icon> <template v-if="showname==null || showname==true">固定试题</template></span>
        </el-tooltip>  
        <slot></slot>    
    </span> `
});