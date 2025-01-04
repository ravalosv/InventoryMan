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
    productId: "00bb8276-4385-4c5a-b0bf-63b5181c83aa",
    sourceStoreId: "15617380-1ce6-4a3d-9214-81dc79b6557b",
    targetStoreId: "1390508a-e79a-451f-8be7-975a9b6a2283",
    quantity: 1
};

export default function() {
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(
        'https://localhost:5001/api/inventory/transfer',
        JSON.stringify(payload),
        params
    );

    check(res, {
        'is status 200': (r) => r.status === 200,
        'response time < 2000ms': (r) => r.timings.duration < 2000,
    });

    sleep(0.1);
}
