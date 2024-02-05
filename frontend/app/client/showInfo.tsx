'use client'

import React, { useState } from 'react';
import { RadioGroup } from '@headlessui/react';
import { CheckIcon } from '@radix-ui/react-icons';

export type DataFormat = {
    date: string,
    temperatureC: number,
    temperatureF: number,
    summary: string
}

const ShowInfo = ({ fetchedData }: { fetchedData: DataFormat[] }) => {
    const [selected, setSelected] = useState<DataFormat | undefined>(fetchedData?.[0])
    return (
        <RadioGroup value={selected} onChange={setSelected}>
            {fetchedData.map((item, i) => (
                <RadioGroup.Option
                    key={i}
                    value={item}
                    className={({ active, checked }) => `item-option ${active ? 'active' : ''} ${checked ? 'checked': ''}`}
                >

                    {({ active, checked }) => (
                    <>
                        <div className="flex w-full items-center justify-between">
                            <div className="flex items-center">
                                <div className="text-sm">
                                <RadioGroup.Label
                                    as="p"
                                    className='item-label'
                                >
                                    {item.date}
                                </RadioGroup.Label>
                                <RadioGroup.Description
                                    as="span"
                                    className='item-desc'
                                >
                                    <span>
                                        {item.temperatureC} ℃ / {item.temperatureF} ℉
                                    </span>
                                    <span>
                                        {item.summary}
                                    </span>
                                </RadioGroup.Description>
                                </div>
                            </div>
                            {checked && (
                                <div className="item-checked-icon">
                                    <CheckIcon className="h-6 w-6" />
                                </div>
                            )}
                        </div>
                    </>
                    )}

                </RadioGroup.Option>
            ))}
        </RadioGroup>
    )
};

export default ShowInfo;