import api from "@/api"

import type { GetLotsListParams } from "@/types/catalog"
import type { CreateLotPayload, CreateLotResponse } from "@/types/createLot"
import type { Lot, LotPreview } from "@/types/lot"
import type { PagedResult } from "@/types/pagination"

function appendValues(
  params: URLSearchParams,
  key: string,
  values?: string[],
): void {
  if (!values?.length) {
    return
  }

  for (const value of values) {
    const normalizedValue = value.trim()

    if (normalizedValue) {
      params.append(key, normalizedValue)
    }
  }
}

function appendBoolean(
  params: URLSearchParams,
  key: string,
  value?: boolean,
): void {
  if (value === true) {
    params.set(key, "true")
  }
}

function appendNumber(
  params: URLSearchParams,
  key: string,
  value?: number,
): void {
  if (value !== undefined) {
    params.set(key, String(value))
  }
}

function buildLotsQueryParams(request: GetLotsListParams): URLSearchParams {
  const params = new URLSearchParams()

  if (request.search?.trim()) {
    params.set("search", request.search.trim())
  }

  appendValues(params, "departments", request.departments)
  appendValues(params, "genders", request.genders)
  appendValues(params, "categories", request.categories)
  appendValues(params, "rootCategorySlugs", request.rootCategorySlugs)
  appendValues(params, "categorySlugs", request.categorySlugs)
  appendValues(params, "brands", request.brands)
  appendValues(params, "sizes", request.sizes)
  appendValues(params, "conditions", request.conditions)
  appendValues(params, "countries", request.countries)
  appendValues(params, "materialIds", request.materialIds)
  appendValues(params, "primaryColorIds", request.primaryColorIds)

  appendNumber(params, "minPrice", request.minPrice)
  appendNumber(params, "maxPrice", request.maxPrice)
  appendNumber(params, "productionYearFrom", request.productionYearFrom)
  appendNumber(params, "productionYearTo", request.productionYearTo)

  appendBoolean(params, "vintageOnly", request.vintageOnly)
  appendBoolean(params, "hasMeasurements", request.hasMeasurements)
  appendBoolean(params, "hasProofOfOrigin", request.hasProofOfOrigin)
  appendBoolean(params, "endingSoon", request.endingSoon)
  appendBoolean(params, "newlyListed", request.newlyListed)

  if (request.sort) {
    params.set("sort", request.sort)
  }

  params.set("page", String(request.page))
  params.set("pageSize", String(request.pageSize))

  return params
}

export type EditLotPayload = Omit<CreateLotPayload, "proofDocuments"> & {
  proofDocuments?: {
    uploadId: string
  }[]
}

export type GetLotsFilters = {
  search?: string
  category?: string | number
  gender?: string | number
  status?: string | number
  size?: string | number
  brand?: string
  sort?: string
  page?: number
  pageSize?: number
}

export const itemService = {
  async createLot(payload: CreateLotPayload): Promise<CreateLotResponse> {
    const response = await api.post<CreateLotResponse>("/api/items/create", payload)
    return response.data
  },

  async getLot(id: string): Promise<Lot> {
    const response = await api.get<Lot>(`/api/items/${id}`)
    return response.data
  },

  async getLots(request: GetLotsListParams): Promise<PagedResult<LotPreview>> {
    const params = buildLotsQueryParams(request)

    const response = await api.get<PagedResult<LotPreview>>("/api/items", {
      params,
    })

    return response.data
  },

  async getMyLots(): Promise<LotPreview[]> {
    const response = await api.get<LotPreview[]>("/api/items/my")
    return response.data
  },

  async updateLot(id: string, payload: EditLotPayload): Promise<void> {
    await api.patch(`/api/items/${id}/update`, payload)
  },

  async deleteLot(id: string): Promise<void> {
    await api.delete(`/api/items/${id}/delete`)
  },

  async publishLot(id: string): Promise<void> {
    await api.post(`/api/items/${id}/publish`)
  },
}
