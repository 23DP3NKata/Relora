<script setup lang="ts">
import { computed, ref } from 'vue'
import {
  ArrowRightIcon,
  CheckCircleIcon,
  ClockIcon,
  EnvelopeIcon,
  ShieldCheckIcon,
} from '@heroicons/vue/24/outline'

import { supportService } from '@/app/services/supportService'

type SupportForm = {
  email: string
  category: string
  subject: string
  message: string
}

const initialForm = (): SupportForm => ({
  email: '',
  category: '',
  subject: '',
  message: '',
})

const form = ref<SupportForm>(initialForm())

const isSubmitting = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const categories = [
  {
    value: 'Technical issue',
    label: 'Technical issue',
  },
  {
    value: 'Account and security',
    label: 'Account & security',
  },
  {
    value: 'Buying',
    label: 'Buying on Relora',
  },
  {
    value: 'Selling',
    label: 'Selling on Relora',
  },
  {
    value: 'Suggestion',
    label: 'Feedback or suggestion',
  },
  {
    value: 'Other',
    label: 'Something else',
  },
]

const messageCharactersLeft = computed(() => {
  return 5000 - form.value.message.length
})

const isFormValid = computed(() => {
  return (
    form.value.email.trim().length > 0 &&
    form.value.category.length > 0 &&
    form.value.subject.trim().length >= 3 &&
    form.value.message.trim().length >= 10
  )
})

async function submit() {
  if (!isFormValid.value || isSubmitting.value) {
    return
  }

  successMessage.value = ''
  errorMessage.value = ''
  isSubmitting.value = true

  try {
    await supportService.createRequest({
      email: form.value.email.trim(),
      category: form.value.category,
      subject: form.value.subject.trim(),
      message: form.value.message.trim(),
    })

    successMessage.value =
      'Your request has been received. The Relora team will reply to the email address you provided.'

    form.value = initialForm()
  } catch {
    errorMessage.value =
      'We could not send your request. Please review the information and try again.'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <section class="relative overflow-hidden">
    <!-- Decorative background -->
    <div
      class="pointer-events-none absolute inset-x-0 top-0 -z-10 h-[420px] bg-gradient-to-b from-foreground/[0.035] to-transparent"
    />

    <div class="mx-auto max-w-6xl px-4 py-12 sm:px-6 sm:py-16 lg:px-8 lg:py-24">
      <!-- Header -->
      <div class="max-w-3xl">
        <div class="flex items-center gap-3">
          <p
            class="text-[11px] font-semibold uppercase tracking-[0.24em] text-foreground/55"
          >
            Relora support
          </p>
        </div>

        <h1
          class="mt-5 max-w-2xl text-4xl font-semibold tracking-[-0.04em] sm:text-5xl lg:text-6xl"
        >
          How can we help?
        </h1>

        <p
          class="mt-5 max-w-2xl text-sm leading-7 text-foreground/60 sm:text-base"
        >
          Tell us about your question or issue. Our team will review your
          request and contact you by email.
        </p>
      </div>

      <div class="mt-12 grid gap-6 lg:grid-cols-[0.75fr_1.35fr] lg:gap-8">
        <!-- Information panel -->
        <aside
          class="flex flex-col justify-between rounded-[28px] border border-border bg-foreground p-7 text-background sm:p-8"
        >
          <div>
            <p
              class="text-[11px] font-semibold uppercase tracking-[0.22em] text-background/50"
            >
              Before contacting us
            </p>

            <h2 class="mt-4 text-2xl font-semibold tracking-tight">
              Include as much detail as possible
            </h2>

            <p class="mt-4 text-sm leading-7 text-background/65">
              Information such as an order number, listing title or a clear
              description will help us resolve your request faster.
            </p>

            <div class="mt-9 space-y-6">
              <div class="flex gap-4">
                <div
                  class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full border border-background/15 bg-background/10"
                >
                  <ClockIcon class="h-5 w-5" />
                </div>

                <div>
                  <p class="text-sm font-medium">Response time</p>
                  <p class="mt-1 text-sm leading-6 text-background/55">
                    Most requests are answered within 1–2 business days.
                  </p>
                </div>
              </div>

              <div class="flex gap-4">
                <div
                  class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full border border-background/15 bg-background/10"
                >
                  <ShieldCheckIcon class="h-5 w-5" />
                </div>

                <div>
                  <p class="text-sm font-medium">Account security</p>
                  <p class="mt-1 text-sm leading-6 text-background/55">
                    Never include passwords or payment card information.
                  </p>
                </div>
              </div>

              <div class="flex gap-4">
                <div
                  class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full border border-background/15 bg-background/10"
                >
                  <EnvelopeIcon class="h-5 w-5" />
                </div>

                <div>
                  <p class="text-sm font-medium">Email updates</p>
                  <p class="mt-1 text-sm leading-6 text-background/55">
                    Our reply will be sent to the address entered in the form.
                  </p>
                </div>
              </div>
            </div>
          </div>

          <p
            class="mt-12 border-t border-background/15 pt-6 text-xs leading-6 text-background/45"
          >
            For an existing order, use the same email address associated with
            your Relora account.
          </p>
        </aside>

        <!-- Form -->
        <div
          class="rounded-[28px] border border-border bg-background p-5 shadow-sm sm:p-8 lg:p-10"
        >
          <div class="mb-8">
            <p class="text-lg font-semibold tracking-tight">Send a request</p>

            <p class="mt-2 text-sm leading-6 text-foreground/55">
              All fields are required.
            </p>
          </div>

          <form class="space-y-6" @submit.prevent="submit">
            <div class="grid gap-6 sm:grid-cols-2">
              <!-- Email -->
              <div class="sm:col-span-1">
                <label
                  for="support-email"
                  class="mb-2.5 block text-sm font-medium"
                >
                  Email address
                </label>

                <input
                  id="support-email"
                  v-model.trim="form.email"
                  type="email"
                  required
                  maxlength="320"
                  autocomplete="email"
                  placeholder="you@example.com"
                  class="h-12 w-full rounded-2xl border border-border bg-transparent px-4 text-sm outline-none transition placeholder:text-foreground/35 hover:border-foreground/30 focus:border-foreground focus:ring-1 focus:ring-foreground"
                />
              </div>

              <!-- Category -->
              <div class="sm:col-span-1">
                <label
                  for="support-category"
                  class="mb-2.5 block text-sm font-medium"
                >
                  Category
                </label>

                <select
                  id="support-category"
                  v-model="form.category"
                  required
                  class="h-12 w-full appearance-none rounded-2xl border border-border bg-background px-4 text-sm outline-none transition hover:border-foreground/30 focus:border-foreground focus:ring-1 focus:ring-foreground"
                >
                  <option disabled value="">Select a category</option>

                  <option
                    v-for="category in categories"
                    :key="category.value"
                    :value="category.value"
                  >
                    {{ category.label }}
                  </option>
                </select>
              </div>
            </div>

            <!-- Subject -->
            <div>
              <div class="mb-2.5 flex items-center justify-between gap-4">
                <label for="support-subject" class="text-sm font-medium">
                  Subject
                </label>

                <span class="text-xs text-foreground/40">
                  {{ form.subject.length }}/200
                </span>
              </div>

              <input
                id="support-subject"
                v-model.trim="form.subject"
                type="text"
                required
                minlength="3"
                maxlength="200"
                placeholder="Briefly describe your request"
                class="h-12 w-full rounded-2xl border border-border bg-transparent px-4 text-sm outline-none transition placeholder:text-foreground/35 hover:border-foreground/30 focus:border-foreground focus:ring-1 focus:ring-foreground"
              />
            </div>

            <!-- Message -->
            <div>
              <div class="mb-2.5 flex items-center justify-between gap-4">
                <label for="support-message" class="text-sm font-medium">
                  Message
                </label>

                <span class="text-xs text-foreground/40">
                  {{ messageCharactersLeft }} characters left
                </span>
              </div>

              <textarea
                id="support-message"
                v-model.trim="form.message"
                required
                minlength="10"
                maxlength="5000"
                rows="8"
                placeholder="Describe what happened, when it happened and what you need help with."
                class="w-full resize-y rounded-2xl border border-border bg-transparent px-4 py-3.5 text-sm leading-6 outline-none transition placeholder:text-foreground/35 hover:border-foreground/30 focus:border-foreground focus:ring-1 focus:ring-foreground"
              />
            </div>

            <!-- Success -->
            <div
              v-if="successMessage"
              role="status"
              aria-live="polite"
              class="flex items-start gap-3 rounded-2xl border border-green-600/15 bg-green-500/[0.07] px-4 py-4 text-sm text-green-800 dark:text-green-300"
            >
              <CheckCircleIcon class="mt-0.5 h-5 w-5 shrink-0" />

              <p class="leading-6">
                {{ successMessage }}
              </p>
            </div>

            <!-- Error -->
            <div
              v-if="errorMessage"
              role="alert"
              class="rounded-2xl border border-red-600/15 bg-red-500/[0.07] px-4 py-4 text-sm leading-6 text-red-700 dark:text-red-300"
            >
              {{ errorMessage }}
            </div>

            <!-- Footer -->
            <div
              class="flex flex-col gap-4 border-t border-border pt-6 sm:flex-row sm:items-center sm:justify-between"
            >
              <p class="max-w-sm text-xs leading-5 text-foreground/45">
                By submitting this form, you agree that Relora may contact you
                regarding this request.
              </p>

              <button
                type="submit"
                :disabled="isSubmitting || !isFormValid"
                class="group inline-flex h-12 w-full items-center justify-center gap-2 rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-85 disabled:cursor-not-allowed disabled:opacity-40 sm:w-auto"
              >
                <span>
                  {{ isSubmitting ? 'Sending request…' : 'Send request' }}
                </span>

                <ArrowRightIcon
                  v-if="!isSubmitting"
                  class="h-4 w-4 transition-transform group-hover:translate-x-0.5"
                />

                <span
                  v-else
                  class="h-4 w-4 animate-spin rounded-full border-2 border-background/30 border-t-background"
                />
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </section>
</template>