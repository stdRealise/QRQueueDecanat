const counterNumberKey = 'counterNumber'

export function getCounterNumber() {
  const storedValue = localStorage.getItem(counterNumberKey)
  if (storedValue === null) {
    return null
  }
  const counterNumber = Number(storedValue)
  if (!Number.isInteger(counterNumber) ||
    counterNumber <= 0) {
    return null
  }
  return counterNumber
}

export function setCounterNumber(counterNumber) {
  if (!Number.isInteger(counterNumber) ||
    counterNumber <= 0) {
    throw new Error('Invalid counter number.')
  }
  localStorage.setItem(counterNumberKey,
    String(counterNumber))
}

export function clearCounterNumber() {
  localStorage.removeItem(counterNumberKey)
}