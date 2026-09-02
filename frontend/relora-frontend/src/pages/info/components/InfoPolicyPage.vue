<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'

import { useLocalePath } from '@/composables/useLocalePath'

type InfoAction = {
  labelKey: string
  to?: string
  eventName?: string
  variant?: 'primary' | 'secondary'
}

type StepGroup = {
  itemsKey: string
  eyebrowKey: string
  titleKey: string
}

type InfoStep = {
  number: string
  title: string
  description: string
}

type InfoSection = {
  id: string
  title: string
  intro?: string
  content?: string[]
  list?: string[]
  note?: string
  email?: string
}

const props = withDefaults(
  defineProps<{
    pageKey: string
    actions?: InfoAction[]
    footerAction?: InfoAction
    stepGroups?: StepGroup[]
    scrollableNav?: boolean
  }>(),
  {
    actions: () => [],
    footerAction: undefined,
    stepGroups: () => [],
    scrollableNav: false,
  },
)

const { t, tm } = useI18n()
const localePath = useLocalePath()

const sections = computed<InfoSection[]>(() =>
  readArray<InfoSection>(`${props.pageKey}.sections`),
)

const summaryBullets = computed<string[]>(() =>
  readArray<string>(`${props.pageKey}.summaryBullets`),
)

const resolvedStepGroups = computed(() =>
  props.stepGroups.map((group) => ({
    ...group,
    steps: readArray<InfoStep>(group.itemsKey),
  })),
)

function readArray<T>(key: string): T[] {
  const value = tm(key)

  return Array.isArray(value) ? (value as T[]) : []
}

function pageMessage(key: string): string {
  return t(`${props.pageKey}.${key}`)
}

function actionLabel(action: InfoAction): string {
  return t(action.labelKey)
}

function actionClasses(action: InfoAction): string {
  if (action.variant === 'secondary') {
    return 'border px-6 text-foreground hover:bg-neutral-100 dark:hover:bg-neutral-800'
  }

  return 'bg-foreground px-6 text-background hover:opacity-80'
}

function dispatchAction(action: InfoAction): void {
  if (!action.eventName) {
    return
  }

  window.dispatchEvent(new CustomEvent(action.eventName))
}
</script>

<template>
  <div class="bg-background text-foreground">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="overflow-hidden rounded-[32px] bg-neutral-100 dark:bg-neutral-800">
        <div
          class="grid gap-8 px-6 py-10 sm:px-10 lg:grid-cols-[1.1fr_0.9fr] lg:px-12 xl:px-16"
        >
          <div class="max-w-3xl">
            <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
              {{ pageMessage('eyebrow') }}
            </p>

            <h1
              class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
            >
              {{ pageMessage('title') }}
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              {{ $t('info.lastUpdated', { date: pageMessage('updatedDate') }) }}
            </p>

            <p class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base">
              {{ pageMessage('description') }}
            </p>

            <div
              v-if="actions.length"
              class="mt-7 flex flex-wrap gap-3"
            >
              <RouterLink
                v-for="action in actions.filter((item) => item.to)"
                :key="action.labelKey"
                :to="localePath(action.to ?? '/')"
                class="inline-flex min-h-11 items-center justify-center rounded-full text-sm font-medium transition"
                :class="actionClasses(action)"
              >
                {{ actionLabel(action) }}
              </RouterLink>

              <button
                v-for="action in actions.filter((item) => item.eventName)"
                :key="action.labelKey"
                type="button"
                class="inline-flex min-h-11 items-center justify-center rounded-full text-sm font-medium transition"
                :class="actionClasses(action)"
                @click="dispatchAction(action)"
              >
                {{ actionLabel(action) }}
              </button>
            </div>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
                {{ pageMessage('summaryTitle') }}
              </p>

              <p class="mt-3 text-sm leading-7 text-foreground/65">
                {{ pageMessage('summary') }}
              </p>

              <div
                v-if="summaryBullets.length"
                class="mt-5 space-y-3"
              >
                <div
                  v-for="item in summaryBullets"
                  :key="item"
                  class="flex items-start gap-3"
                >
                  <span class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70" />

                  <span class="text-sm leading-6 text-foreground/60">
                    {{ item }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section
      v-for="group in resolvedStepGroups"
      :key="group.itemsKey"
      class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8"
    >
      <div class="mb-8">
        <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
          {{ $t(group.eyebrowKey) }}
        </p>

        <h2 class="mt-3 text-3xl font-semibold tracking-tight text-foreground">
          {{ $t(group.titleKey) }}
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="step in group.steps"
          :key="`${group.itemsKey}-${step.number}`"
          class="rounded-[28px] border bg-background p-6"
        >
          <span class="text-xs font-medium tracking-[0.18em] text-foreground/40">
            {{ step.number }}
          </span>

          <h3 class="mt-8 text-lg font-semibold tracking-tight text-foreground">
            {{ step.title }}
          </h3>

          <p class="mt-3 text-sm leading-7 text-foreground/60">
            {{ step.description }}
          </p>
        </article>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="grid gap-8 lg:grid-cols-[260px_minmax(0,1fr)]">
        <aside class="hidden lg:block">
          <div class="sticky top-36 rounded-[28px] border bg-background p-5">
            <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
              {{ $t('info.onThisPage') }}
            </p>

            <nav
              class="mt-4 space-y-3 pr-2"
              :class="{ 'max-h-[calc(100vh-12rem)] overflow-y-auto': scrollableNav }"
            >
              <a
                v-for="section in sections"
                :key="section.id"
                :href="`#${section.id}`"
                class="block text-sm text-foreground/60 transition hover:text-foreground"
              >
                {{ section.title }}
              </a>
            </nav>
          </div>
        </aside>

        <div class="space-y-6">
          <section
            v-for="section in sections"
            :id="section.id"
            :key="section.id"
            class="scroll-mt-40 rounded-[28px] border bg-background p-6 sm:p-8"
          >
            <h2 class="text-2xl font-semibold tracking-tight text-foreground">
              {{ section.title }}
            </h2>

            <p
              v-if="section.intro"
              class="mt-5 text-sm leading-8 text-foreground/65 sm:text-base"
            >
              {{ section.intro }}
            </p>

            <div
              v-if="section.content"
              class="mt-5 space-y-4 text-sm leading-8 text-foreground/65 sm:text-base"
            >
              <p
                v-for="paragraph in section.content"
                :key="paragraph"
              >
                {{ paragraph }}
              </p>
            </div>

            <ul
              v-if="section.list"
              class="mt-5 space-y-3"
            >
              <li
                v-for="item in section.list"
                :key="item"
                class="flex items-start gap-3 text-sm leading-7 text-foreground/65 sm:text-base"
              >
                <span class="mt-2.5 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70" />

                <span>{{ item }}</span>
              </li>
            </ul>

            <div
              v-if="section.note"
              class="mt-6 rounded-[22px] bg-neutral-100 px-5 py-4 dark:bg-neutral-800"
            >
              <p class="text-[11px] uppercase tracking-[0.2em] text-foreground/50">
                {{ $t('info.important') }}
              </p>

              <p class="mt-2 text-sm leading-7 text-foreground/65">
                {{ section.note }}
              </p>
            </div>

            <p
              v-if="section.email"
              class="mt-5 text-sm text-foreground/45"
            >
              {{ $t('info.email') }}: {{ section.email }}
            </p>
          </section>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
      <div class="rounded-[32px] border bg-background px-6 py-10 text-center sm:px-10">
        <p class="text-sm text-foreground/45">
          {{ pageMessage('footerMeta') }}
        </p>

        <p class="mt-3 text-xl font-medium text-foreground/70">
          {{ pageMessage('footerText') }}
        </p>

        <p
          v-if="$te(`${pageKey}.footerDescription`)"
          class="mx-auto mt-3 max-w-xl text-sm leading-7 text-foreground/55"
        >
          {{ pageMessage('footerDescription') }}
        </p>

        <RouterLink
          v-if="footerAction?.to"
          :to="localePath(footerAction.to)"
          class="mt-6 inline-flex min-h-11 items-center justify-center rounded-full text-sm font-medium transition"
          :class="actionClasses(footerAction)"
        >
          {{ actionLabel(footerAction) }}
        </RouterLink>

        <button
          v-else-if="footerAction?.eventName"
          type="button"
          class="mt-6 inline-flex min-h-11 items-center justify-center rounded-full text-sm font-medium transition"
          :class="actionClasses(footerAction)"
          @click="dispatchAction(footerAction)"
        >
          {{ actionLabel(footerAction) }}
        </button>
      </div>
    </section>
  </div>
</template>
