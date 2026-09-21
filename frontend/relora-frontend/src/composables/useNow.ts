import { onBeforeUnmount, onMounted, ref } from 'vue'

// current time, updates every second
export function useNow() {
  const now = ref(Date.now())
  let timer: ReturnType<typeof setInterval> | undefined

  onMounted(() => {
    timer = setInterval(() => {
      now.value = Date.now()
    }, 1000)
  })

  onBeforeUnmount(() => {
    clearInterval(timer)
  })

  return now
}
