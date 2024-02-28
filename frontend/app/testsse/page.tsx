'use client'

import { useContext } from "react"

import { SSEContext } from '../../context/sse'
import { SSELogElement } from '../../components/sse/sselog'


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