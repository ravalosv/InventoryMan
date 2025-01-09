// k6 run --insecure-skip-tls-verify load-test.js


import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 500 },
        { duration: '1m', target: 500 },
        { duration: '30s', target: 0 },
    ],
    thresholds: {
        'http_req_duration': ['p(95)<2000'],
        'http_req_failed': ['rate<0.01'],
    },
};

const payload = {
    productId: "ELEC-CABL-001",
    sourceStoreId: "STR-01",
    targetStoreId: "STR-02",
    quantity: 1
};

export default function() {
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(
        'https://[URLServidorAPI]/api/inventory/transfer',
        JSON.stringify(payload),
        params
    );

    check(res, {
        'is status 200': (r) => r.status === 200,
        'response time < 2000ms': (r) => r.timings.duration < 2000,
    });

    sleep(0.1);
}
