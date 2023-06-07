export const INCREMENT_COUNTER = "INCREMENT_COUNTER";
export const DECREMENR_COUNTER = "DECREMENR_COUNTER";

export interface CounterState {
    data: number;
    title: string;
}

const initialState: CounterState = {
    data: 42,
    title: 'YARC (yet another redux counter)'
}

export function increment(amount = 1){
    return {
        type: INCREMENT_COUNTER,
        payload: amount
    }
}

export function decrement(amount = 1){
    return {
        type: DECREMENR_COUNTER,
        payload: amount
    }
}

export default function counterReducer(state = initialState, action: any) {
    switch (action.type) {
        case INCREMENT_COUNTER:
            return {
                ...state,
                data: state.data + action.payload
            }
        case DECREMENR_COUNTER:
            return {
                ...state,
                data: state.data - action.payload
            }
        default:
            return state;
    }
}