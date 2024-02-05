'use client'

import { ChangeEvent, FormEvent, FormEventHandler, useEffect, useState } from "react";

const SearchForm = ({ onSubmit }: { onSubmit: Function }) => {
    const [inputValue, setInputValue] = useState<string>('');
    const handleInput = (e: ChangeEvent<HTMLInputElement>): undefined => {
        setInputValue(e.target.value);
    };
    const handleSubmit = (e: FormEvent<HTMLFormElement>): undefined => {
        e.preventDefault();
        onSubmit(inputValue);
    };
    return (
        <form onSubmit={handleSubmit}>
          <input type="text" value={inputValue} onChange={handleInput} />
          <button type="submit">Submit</button>
        </form>
    );
}

export default SearchForm;