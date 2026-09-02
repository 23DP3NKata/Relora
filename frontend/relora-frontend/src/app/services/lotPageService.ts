import type { Lot } from '@/types/lot.ts'
import api from '@/api'

export function LotPageService() {
    async function getlotInformation(lotId: string): Promise<Lot> 
    {
        const response = await api.get(`/api/items/${lotId}`)
        return response.data    
    }

   return { getlotInformation }
}