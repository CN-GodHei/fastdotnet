<template>
  <component :is="type" v-bind="linkProps(to)">
    <slot />
  </component>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { isExternal } from '@/utils/validate'

interface Props {
  to: string
}

const props = defineProps<Props>()

// 链接类型
const type = computed(() => {
  if (isExternal(props.to)) {
    return 'a'
  }
  return 'router-link'
})

// 链接属性
const linkProps = (to: string) => {
  if (isExternal(to)) {
    return {
      href: to,
      target: '_blank',
      rel: 'noopener'
    }
  }
  return {
    to: to
  }
}
</script>