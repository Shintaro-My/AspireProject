'use client'

import { ButtonHTMLAttributes } from 'react'
import classNames from "classnames"
import { Switch, SwitchProps } from '@headlessui/react'

import './checkbox.scss'

type Props = {
    checked?: boolean,
    onChange?: (checked: boolean) => void,
}

const CustomCheckBox = ({ checked, onChange, className }: SwitchProps<'button'>) => {
    return (
        <Switch checked={checked} onChange={onChange} className={classNames(className, 'custom_check', 'simple', checked ? 'checked' : '')}>
            <div className={classNames(className, 'custom_check_toggle', checked ? 'checked' : '')} area-hidden='true'></div>
        </Switch>
    )
}

export default CustomCheckBox