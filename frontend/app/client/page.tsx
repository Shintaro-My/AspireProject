'use client'

import { useEffect, useState } from "react";

const getData = async () => {

    const weatherData: Response = await fetch('/api/weatherforecast', { cache: 'no-cache' });

    if (!weatherData.ok) {
        throw new Error('Failed to fetch data.');
    }

    const data = await weatherData.json();

    return data
}

const Page = () => {

    const [data, setData] = useState([])

    useEffect(() => {
        getData().then(setData)
    }, [])

    return <main>{JSON.stringify(data)}</main>
}

export default Page
