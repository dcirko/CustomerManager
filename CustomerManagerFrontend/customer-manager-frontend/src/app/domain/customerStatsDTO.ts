import { Top5CitiesDTO } from "./top5CitiesDTO";

export interface CustomerStatsDTO{
    totalCount: number,
    activeCount: number,
    inactiveCount: number,
    top5Cities: Top5CitiesDTO[]
}