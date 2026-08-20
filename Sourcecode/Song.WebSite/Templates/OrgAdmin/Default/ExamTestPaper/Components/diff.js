//难度选择
Vue.component('diff', {
    props: ['value'],
    data: function () {
        return {
            diffs: ['很简单', '简单', '有难度', '比较难', '很难'],
            loading: false
        }
    },
    watch: {

    },
    computed: {
        label: function () {
            let v = Number(this.value == null || this.value == '' ? -1 : this.value);
            if (v < 0 || v > 5) return '难度';
            return this.diffs[v - 1];
        },
    },
    mounted: function () {

    },
    methods: {
        handleCommand: function (command) {
            this.value = command + 1;
            this.$emit('change', this.value);
        }
    },
    template: `<div class="ques_diff">  
        <el-dropdown @command="handleCommand">
            <span class="el-dropdown-link">
            {{label}}<i class="el-icon-arrow-down el-icon--right"></i>
            </span>
            <el-dropdown-menu slot="dropdown">
                <el-dropdown-item :command="-1" >-- 所有 --</el-dropdown-item>
                <el-dropdown-item v-for="(item,idx) in diffs" :command="idx" :divided="idx==0">
                    {{item}}
                </el-dropdown-item>          
            </el-dropdown-menu>
        </el-dropdown>
    </div> `
});