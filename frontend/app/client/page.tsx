'use client'

import { useEffect, useState } from "react"
import ShowInfo, { DataFormat } from "./showInfo"
import SearchForm from "./searchForm"

const getData = async (): Promise<DataFormat[]> => {
    const weatherData: Response = await fetch('/api/weatherforecast', { cache: 'no-cache' })
    if (!weatherData.ok) {
        throw new Error('Failed to fetch data.')
    }
    const data = await weatherData.json()
    return data
}

const ShowPage = ({ initialData }: { initialData?: DataFormat[] }) => {
    const [fetchedData, setFetchedData] = useState<DataFormat[]>(initialData ?? [])
    const handleFormSubmit = async (inputValue: string) => {
        const data: DataFormat[] = await getData()
        setFetchedData(data)
    }
    return (
        <>
            <SearchForm onSubmit={handleFormSubmit}></SearchForm>
            <ShowInfo fetchedData={fetchedData}></ShowInfo>
        </>
    )
}

export default ShowPage