'use client'

import { useContext } from "react"

import { SSEContext, SSELogElement } from '../components/context/sse'


const TestSSEPage = () => {
    const sseContext = useContext(SSEContext)
    const ping = sseContext?.ping
    const msgs = sseContext?.messages ?? []

    return (
        <>
        <SSELogElement />
        </>
    )
}

export default TestSSEPage