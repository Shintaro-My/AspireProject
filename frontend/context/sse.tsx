'use client'

import { Dispatch, SetStateAction, createContext, useContext, useState } from "react"


type SSESetter = {
    // source: EventSource | null,
    // setSource: Dispatch<SetStateAction<EventSource | null>>,
    messages: SSEMessage[],
    ping: number | null
}

export const SSEContext = createContext<SSESetter | null>(null)

export type SSEMessage = {
    id?: string,
    type: string,
    message: any
}

type Props = {
    isValid: boolean,
    children: React.ReactNode
}
export const SSEProvider = ({ children, isValid }: Props) => {
    const [messages, setMessages] = useState<SSEMessage[]>([])
    const [ping, setPing] = useState<number | null>(null)

    const [source, setSource] = useState<EventSource | null>(null)

    if (isValid) {
        if (!source) {
            const sse = new EventSource('/sse/default');
            sse.addEventListener('message', (e: MessageEvent) => {
                setMessages([JSON.parse(e.data), ...messages].slice(0, 250))
            })
            sse.addEventListener('ping', (e: MessageEvent) => {
                setPing(JSON.parse(e.data))
            })
            console.log('SSEProvider: active')
            setSource(sse)
        }
    }
    else {
        if (source) {
            source.close()
            console.log('SSEProvider: inactive')
            setSource(null)
        }
    }
    const value: SSESetter = { messages, ping }
    return <SSEContext.Provider value={value}>{children}</SSEContext.Provider>
}

