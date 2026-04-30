<template>
  <div class="chart-widget-container" ref="chartRef"></div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue';
import * as echarts from 'echarts';

const props = defineProps({
  data: { type: [Array, Object], default: () => [] },
  config: { type: Object, default: () => ({}) }
});

const chartRef = ref<HTMLElement | null>(null);
let myChart: echarts.ECharts | null = null;

// 1. 初始化图表
const initChart = () => {
  if (!chartRef.value) return;
  
  if (myChart) {
    myChart.dispose();
  }
  
  myChart = echarts.init(chartRef.value);
  updateOptions();
};

// 2. 根据数据和配置更新图表
const updateOptions = () => {
  if (!myChart) return;
  
  const { 
    chartType = 'bar', 
    title = '', 
    xAxisField = 'name', 
    valueField = 'value',
    nameField = 'name',
    smooth = true,
    color = '#409EFF'
  } = props.config;

  // 处理数据
  let seriesData: any[] = [];
  let xAxisData: any[] = [];

  if (Array.isArray(props.data)) {
    if (chartType === 'pie') {
      seriesData = props.data.map(item => ({
        name: item[nameField] || item.Name || '未知',
        value: item[valueField] || item.Value || 0
      }));
    } else {
      xAxisData = props.data.map(item => item[xAxisField] || item.Name || '');
      seriesData = props.data.map(item => item[valueField] || item.Value || 0);
    }
  }

  const option: echarts.EChartsOption = {
    title: {
      text: title,
      textStyle: { fontSize: 14, fontWeight: 'normal' },
      left: 'center',
      top: 10
    },
    tooltip: { trigger: chartType === 'pie' ? 'item' : 'axis' },
    grid: { left: '10%', right: '10%', bottom: '15%', top: '20%', containLabel: true },
    xAxis: chartType === 'pie' ? undefined : {
      type: 'category',
      data: xAxisData,
      axisLabel: { interval: 0, rotate: 30 }
    },
    yAxis: chartType === 'pie' ? undefined : { type: 'value' },
    series: [
      {
        data: seriesData,
        type: chartType,
        smooth: smooth,
        itemStyle: { color: color },
        radius: chartType === 'pie' ? ['40%', '70%'] : undefined,
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        }
      }
    ]
  };

  myChart.setOption(option);
};

// 3. 监听变化
watch(() => props.data, () => {
  updateOptions();
}, { deep: true });

watch(() => props.config, () => {
  updateOptions();
}, { deep: true });

// 4. 自适应大小
const handleResize = () => {
  myChart?.resize();
};

onMounted(() => {
  nextTick(() => {
    initChart();
    window.addEventListener('resize', handleResize);
  });
});

onUnmounted(() => {
  window.removeEventListener('resize', handleResize);
  myChart?.dispose();
});
</script>

<style scoped lang="scss">
.chart-widget-container {
  width: 100%;
  height: 100%;
  min-height: 200px;
}
</style>
