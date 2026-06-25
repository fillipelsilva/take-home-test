# Frontend

Angular application for the Loan Management System.

## Features

- Table listing all loans from the API
- **Add Loan** button reveals a create form
- **Edit** (pencil icon) loads the same form with loan data
- **Delete** (trash icon) opens a confirmation dialog with Cancel and Confirm
- Automatic JWT authentication against the backend

## Prerequisites

- [Node.js 20+](https://nodejs.org/)
- Backend API running at http://localhost:5000 (see `backend/src/README.md`)

## Running the Frontend

Install dependencies:

```sh
npm install
```

Start the development server:

```sh
npm start
```

Open http://localhost:4200 in your browser.

The app authenticates with the `LoanAdmin` role and loads loans from `GET /loans`.

## API Configuration

The API base URL is configured in:

- `src/environments/environment.ts` (development)
- `src/environments/environment.prod.ts` (production)

Default: `http://localhost:5000`

## Build

```sh
npm run build
```

Output is written to `dist/temp`.
