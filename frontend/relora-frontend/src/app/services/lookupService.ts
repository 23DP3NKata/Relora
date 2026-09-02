import api from "@/api"

import type {
  LotFormLookups,
  MeasurementDefinition,
} from "@/types/lotCatalog"

export const lookupService = {
  async getLotFormLookups(): Promise<LotFormLookups> {
    const response = await api.get<LotFormLookups>("/api/lookups/lot-form")
    return response.data
  },

  async getMeasurementSchema(categoryId: string): Promise<MeasurementDefinition[]> {
    const response = await api.get<MeasurementDefinition[]>(
      `/api/lookups/categories/${categoryId}/measurements`,
    )

    return response.data
  },
}
