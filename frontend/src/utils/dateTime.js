const currentDateTimeFormatter = new Intl.DateTimeFormat(
  'ru-RU',
  {
    timeZone: 'Asia/Yekaterinburg',
    dateStyle: 'short',
    timeStyle: 'short',
  },
)

const currentTimeFormatter = new Intl.DateTimeFormat(
  'ru-RU',
  {
    timeZone: 'Asia/Yekaterinburg',
    hour: '2-digit',
    minute: '2-digit',
  },
)

const dateFormatter = new Intl.DateTimeFormat(
  'ru-RU',
  {
    timeZone: 'Asia/Yekaterinburg',
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  },
)

export function formatDateTime(value) {
  if (!value) {
    return '—'
  }
  return currentDateTimeFormatter.format(new Date(value))
}

export function formatTime(value) {
  if (!value) {
    return '—'
  }
  return currentTimeFormatter.format(new Date(value))
}

export function formatDate(value) {
  if (!value) {
    return '—'
  }
  return dateFormatter.format(new Date(value))
}